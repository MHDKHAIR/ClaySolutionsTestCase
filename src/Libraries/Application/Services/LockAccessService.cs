using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DataTransfareObjects.Requests;
using Application.Validators;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using Application.Services.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;
using Application.DataTransfareObjects.Responses;

namespace Application.Services
{
    public class LockAccessService : ILockAccessService
    {
        #region dependency
        readonly ICurrentUserService _currentUserService;
        readonly IGeoLocationService _locationService;
        readonly ILockControlService _lockControlService;
        readonly IUserService _userService;
        readonly IDateTimeService _dateTimeService;
        readonly IEfRepository<DoorLockEntity> _lockRepo;
        readonly IEfRepository<LockAccessHistoryEntity> _accessHistoryRepo;
        readonly IEfRepository<UserLockClaimEntity> _userLockClaimRepo;
        readonly INotificationService _notificationService;
        #endregion
        #region constructor
        public LockAccessService(
            ICurrentUserService currentUserService,
            IGeoLocationService locationService,
            ILockControlService lockControlService,
            IUserService userService,
            IDateTimeService dateTimeService,
            IEfRepository<DoorLockEntity> lockRepo,
            IEfRepository<LockAccessHistoryEntity> accessHistoryRepo,
            IEfRepository<UserLockClaimEntity> userLockClaimRepo,
            INotificationService notificationService)
        {
            _currentUserService = currentUserService;
            _locationService = locationService;
            _lockControlService = lockControlService;
            _userService = userService;
            _dateTimeService = dateTimeService;
            _lockRepo = lockRepo;
            _accessHistoryRepo = accessHistoryRepo;
            _userLockClaimRepo = userLockClaimRepo;
            _notificationService = notificationService;
        }
        #endregion
        #region public
        public virtual async Task<AccessLockResponseDto> AccessLock(AccessLockRequestDto requestDto)
        {
            //validation
            var validator = new LockAccessrRequestValidator();
            var valdationResult = validator.Validate(requestDto);
            valdationResult.Resolve();
            //check lock
            var thisLock = await CheckLockByCodeAsync(requestDto.DoorKeyCode);
            //check distance
            await CheckDistance(thisLock, requestDto);
            //check claim
            var needConfirm = false;
            var lastLockClaim = await _userLockClaimRepo.FirstByConditionAsync(x => x.LockId == thisLock.Id && x.UserId == _currentUserService.UserId, false);
            if (lastLockClaim is null)
            {
                lastLockClaim = await _userLockClaimRepo.InsertAsync(new UserLockClaimEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    ClaimType = _currentUserService.UserType == UserTypeEnum.Guest ? ClaimTypeEnum.Once : ClaimTypeEnum.Always,
                    LockId = thisLock.Id,
                    UserId = _currentUserService.UserId,
                    AccessUntil = _currentUserService.UserType == UserTypeEnum.Guest ? _dateTimeService.Now.AddSeconds(-1) : _dateTimeService.Now.AddYears(1)
                });
                if (_currentUserService.UserType == UserTypeEnum.Guest)
                    needConfirm = true;
            }
            else if (lastLockClaim.AccessUntil < _dateTimeService.Now)
            {
                if (_currentUserService.UserType == UserTypeEnum.Guest)
                    needConfirm = true;
                else
                    lastLockClaim.AccessUntil = _dateTimeService.Now.AddYears(1);
            }
            // notify admin with user access
            var thisUser = await _userService.GetByIdAsync(_currentUserService.UserId);
            _notificationService.GrantAccessNotify(thisUser, lastLockClaim.Id, needConfirm);
            await _accessHistoryRepo.InsertAsync(new LockAccessHistoryEntity
            {
                AccessStatus = needConfirm ? LockAccessStatusEnum.AccessDenied : LockAccessStatusEnum.AccessGranted,
                DoorLockId = thisLock.Id,
                Reason = "Requrested lock access",
                UserId = thisUser.Id,
            });
            // save to db
            await _userLockClaimRepo.SaveChangesAsync();
            await _accessHistoryRepo.SaveChangesAsync();
            if (needConfirm)
                throw new UnauthorizedAccessException("Please wait access will be granted on admin confirmation");
            //IOT service call simulation to open the door
            _ = Task.Run(async () => await _lockControlService.OpenLock(thisLock.DoorKeyCode));
            return new AccessLockResponseDto
            {
                DoorKeyCode = thisLock.DoorKeyCode,
                AccessValidUntil = lastLockClaim.AccessUntil.GetValueOrDefault(),
                ClaimType = lastLockClaim.ClaimType.ToString()
            };
        }
        public async Task GrantAccessOnLock(string claimId)
        {
            if (_currentUserService.UserType is Domain.Enums.UserTypeEnum.Admin)
                throw new UnauthorizedAccessException("This is only for admin");
            //validation
            if (string.IsNullOrEmpty(claimId))
                throw new Common.Exeptions.ApplicationException("ClaimId is required");
            //check claim
            var lockClaim = await _userLockClaimRepo.GetAsync(claimId, false);
            if (lockClaim is null)
                throw new KeyNotFoundException("ClaimId is not valid");
            if (lockClaim.AccessUntil > _dateTimeService.Now)
                throw new Common.Exeptions.ApplicationException("Access already granded");
            //check lock
            var doorLock = await CheckLockByIdAsync(lockClaim.LockId);
            //update claim
            var user = await _userService.GetByIdAsync(lockClaim.UserId);
            lockClaim.AccessUntil = user.UserType == UserTypeEnum.Guest ?
                _dateTimeService.Now.AddHours(1) : _dateTimeService.Now.AddYears(1);
            await _userLockClaimRepo.SaveChangesAsync();
            // update history
            var history = await _accessHistoryRepo.FirstByConditionAsync(x => x.UserId == lockClaim.UserId && x.DoorLockId == lockClaim.LockId, AsNoTracking: false);
            if (history is not null)
            {
                history.AccessStatus = LockAccessStatusEnum.AccessGranted;
                await _accessHistoryRepo.SaveChangesAsync();
            }
            //IOT service call simulation to open the door
            _ = Task.Run(async () => await _lockControlService.OpenLock(doorLock.DoorKeyCode));
        }
        #endregion
        #region private
        async Task CheckDistance(DoorLockEntity thisLock, AccessLockRequestDto requestDto)
        {
            var userFromLockDistance = _locationService.CalculateDistance(thisLock.Latitude, requestDto.Location.Latitude,
                      thisLock.Longitude, requestDto.Location.Longitude);
            if (userFromLockDistance > (await _lockControlService.GetLockValidDistance(thisLock.DoorKeyCode)))
                throw new Common.Exeptions.ApplicationException("Too far to access the lock, the distance should be 10 or less meters");
        }
        async Task<DoorLockEntity> CheckLockByIdAsync(Guid id)
        {
            var thisLock = await _lockRepo.FirstByConditionAsync(l => l.Id == id && l.RecordStatus != RecordStatusEnum.Deleted, true);
            if (thisLock is null)
                throw new KeyNotFoundException("Lock does not exist");
            if (thisLock.RecordStatus == RecordStatusEnum.InActive)
                throw new Common.Exeptions.ApplicationException("Lock does not active");
            return thisLock;
        }
        async Task<DoorLockEntity> CheckLockByCodeAsync(string code)
        {
            var thisLock = await _lockRepo.FirstByConditionAsync(l => l.DoorKeyCode.Equals(code) && l.RecordStatus != RecordStatusEnum.Deleted, true);
            if (thisLock is null)
                throw new KeyNotFoundException("DoorKeyCode does not exist");
            if (thisLock.RecordStatus == RecordStatusEnum.InActive)
                throw new Common.Exeptions.ApplicationException("DoorKeyCode does not active");
            return thisLock;
        }
        #endregion
    }
}