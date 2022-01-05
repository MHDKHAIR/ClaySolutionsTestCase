using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DataTransfareObjects.Requests;
using Application.Validators;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using Domain.Common;
using Microsoft.Extensions.Configuration;
using Application.Services.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;

namespace Application.Services
{
    public class LockAccessService : ILockAccessService
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IGeoLocationService locationService;
        private readonly ILockControlService lockControlService;
        private readonly IEmailService emailService;
        private readonly IUserService userService;
        private readonly IDateTimeService dateTimeService;
        private readonly IReadRepository<DoorLockEntity> lockRepo;
        private readonly IReadRepository<LockAccessHistoryEntity> accessHistoryReadRepo;
        private readonly IWriteRepository<LockAccessHistoryEntity> accessHistoryWriteRepo;
        private readonly IReadRepository<UserLockClaimEntity> userLockClaimReadRepo;
        private readonly IWriteRepository<UserLockClaimEntity> userLockClaimWriteRepo;
        private readonly IConfiguration configuration;

        public LockAccessService(
            ICurrentUserService currentUserService,
            IGeoLocationService locationService,
            ILockControlService lockControlService,
            IEmailService emailService,
            IUserService userService,
            IDateTimeService dateTimeService,
            IReadRepository<DoorLockEntity> lockRepo,
            IReadRepository<LockAccessHistoryEntity> accessHistoryReadRepo,
            IWriteRepository<LockAccessHistoryEntity> accessHistoryWriteRepo,
            IReadRepository<UserLockClaimEntity> userLockClaimReadRepo,
            IWriteRepository<UserLockClaimEntity> userLockClaimWriteRepo,
            IConfiguration configuration)
        {
            this.currentUserService = currentUserService;
            this.locationService = locationService;
            this.lockControlService = lockControlService;
            this.emailService = emailService;
            this.userService = userService;
            this.dateTimeService = dateTimeService;
            this.lockRepo = lockRepo;
            this.accessHistoryReadRepo = accessHistoryReadRepo;
            this.accessHistoryWriteRepo = accessHistoryWriteRepo;
            this.userLockClaimReadRepo = userLockClaimReadRepo;
            this.userLockClaimWriteRepo = userLockClaimWriteRepo;
            this.configuration = configuration;
        }

        public async Task AccessLock(AccessLockRequestDto requestDto)
        {
            //validation
            var validator = new LockAccessrRequestValidator();
            var valdationResult = validator.Validate(requestDto);
            valdationResult.Resolve();

            //check lock
            var thisLock = await CheckLockByCodeAsync(requestDto.DoorKeyCode);
            var userFromLockDistance = locationService.CalculateDistance(thisLock.Latitude, requestDto.Location.Latitude,
                       thisLock.Longitude, requestDto.Location.Longitude);

            //check distance
            if (userFromLockDistance > (await lockControlService.GetLockValidDistance(thisLock.DoorKeyCode)))
                throw new ApplicationException("Too far to access the lock, the distance should be 10 or less meters");

            //check claim
            var needConfirm = false;

            var lastLockClaim = await userLockClaimReadRepo.FirstByConditionAsync(x => x.LockId == thisLock.Id && x.UserId == currentUserService.UserId, AsNoTracking: false);
            if (lastLockClaim is null)
            {
                lastLockClaim = await userLockClaimWriteRepo.InsertAsync(new UserLockClaimEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    ClaimType = currentUserService.UserType == UserTypeEnum.Guest ? ClaimTypeEnum.Once : ClaimTypeEnum.Always,
                    LockId = thisLock.Id,
                    UserId = currentUserService.UserId,
                    AccessUntil = currentUserService.UserType == UserTypeEnum.Guest ? dateTimeService.Now.AddSeconds(-1) : dateTimeService.Now.AddYears(1)
                });
                if (currentUserService.UserType == UserTypeEnum.Guest)
                    needConfirm = true;
            }
            else if (lastLockClaim.AccessUntil < dateTimeService.Now)
            {
                if (currentUserService.UserType == UserTypeEnum.Guest)
                    needConfirm = true;
                else
                    lastLockClaim.AccessUntil = dateTimeService.Now.AddYears(1);
            }
            // notify admin with user access
            var thisUser = await userService.GetByIdAsync(currentUserService.UserId);
            NotifyAdminToGrantAccess(thisUser.Email, lastLockClaim.Id, needConfirm);

            await accessHistoryWriteRepo.InsertAsync(new LockAccessHistoryEntity
            {
                AccessStatus = needConfirm ? LockAccessStatusEnum.AccessDenied : LockAccessStatusEnum.AccessGranted,
                DoorLockId = thisLock.Id,
                Reason = "Requrested lock access",
                UserId = thisUser.Id,
            });

            // save to db
            await userLockClaimWriteRepo.SaveChangesAsync();
            await accessHistoryWriteRepo.SaveChangesAsync();

            if (needConfirm)
                throw new UnauthorizedAccessException("Please wait access will be granted on admin confirmation");
            //IOT service call simulation to open the door
            await lockControlService.OpenLock(thisLock.DoorKeyCode);
        }

        public async Task GrantAccessOnLock(string claimId)
        {
            //validation
            if (string.IsNullOrEmpty(claimId))
                throw new ApplicationException("ClaimId is required");

            //check claim
            var lockClaim = await userLockClaimReadRepo.GetAsync(claimId, false);
            if (lockClaim is null)
                throw new KeyNotFoundException("ClaimId is not valid");

            if (lockClaim.AccessUntil > dateTimeService.Now)
                throw new ApplicationException("Access already granded");

            //check lock
            var doorLock = await CheckLockByIdAsync(lockClaim.LockId);

            //update claim
            var user = await userService.GetByIdAsync(lockClaim.UserId);
            lockClaim.AccessUntil = user.UserType == UserTypeEnum.Guest ?
                dateTimeService.Now.AddHours(1) : dateTimeService.Now.AddYears(1);

            await userLockClaimWriteRepo.SaveChangesAsync();

            // update history
            var history = await accessHistoryReadRepo.FirstByConditionAsync(x => x.UserId == lockClaim.UserId && x.DoorLockId == lockClaim.LockId, AsNoTracking: false);
            if (history is not null)
            {
                history.AccessStatus = LockAccessStatusEnum.AccessGranted;
                await accessHistoryWriteRepo.SaveChangesAsync();
            }
            // open the door
            await lockControlService.OpenLock(doorLock.DoorKeyCode);
        }

        #region Private
        async Task<DoorLockEntity> CheckLockByCodeAsync(string code)
        {
            var thisLock = await lockRepo.FirstByConditionAsync(l => l.DoorKeyCode.Equals(code) && l.RecordStatus != RecordStatusEnum.Deleted);
            if (thisLock is null)
                throw new KeyNotFoundException("DoorKeyCode does not exist");
            if (thisLock.RecordStatus == RecordStatusEnum.InActive)
                throw new ApplicationException("DoorKeyCode does not active");
            return thisLock;
        }
        async Task<DoorLockEntity> CheckLockByIdAsync(Guid id)
        {
            var thisLock = await lockRepo.FirstByConditionAsync(l => l.Id == id && l.RecordStatus != RecordStatusEnum.Deleted);
            if (thisLock is null)
                throw new KeyNotFoundException("Lock does not exist");
            if (thisLock.RecordStatus == RecordStatusEnum.InActive)
                throw new ApplicationException("Lock does not active");
            return thisLock;
        }

        // for demo purpose
        void NotifyAdminToGrantAccess(string userEmail, string claimId, bool needConfirm = true)
        {
            var withLink = string.Empty;
            if (currentUserService.UserType == UserTypeEnum.Guest && needConfirm)
            {
                var cofirmationLink = configuration.GetValue<string>("HostDomain");
                cofirmationLink += $"api/admin/grant_access?ClaimId={claimId}";
                withLink = $"</br>Confirm user account link:</br>{cofirmationLink}";
            }

            emailService.SendEmailTask(new MailRequest
            {
                Subject = "User requested door access!",
                ToEmail = configuration.GetValue<string>("AdminEmail"),
                Body = $"{currentUserService.UserType} User email: {userEmail} requested access {withLink}"
            });
        }
        #endregion
    }
}
