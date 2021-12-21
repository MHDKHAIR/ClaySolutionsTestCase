using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataTransfareObjects.Requests;
using Application.Services.Contracts;
using Application.Validators;
using Application.Extentions;
using Domain.Entities;
using Domain.Interfaces.Contexts;
using Domain.Interfaces.Services;
using Domain.Enums;
using Domain.Common;
using Microsoft.Extensions.Configuration;

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
        private readonly IRepository<DoorLockEntity> lockRepo;
        private readonly IRepository<LockAccessHistoryEntity> accessHistoryRepo;
        private readonly IRepository<UserLockClaimEntity> userLockClaimRepo;
        private readonly IConfiguration configuration;

        public LockAccessService(
            ICurrentUserService currentUserService,
            IGeoLocationService locationService,
            ILockControlService lockControlService,
            IEmailService emailService,
            IUserService userService,
            IDateTimeService dateTimeService,
            IRepository<DoorLockEntity> lockRepo,
            IRepository<LockAccessHistoryEntity> accessHistoryRepo,
            IRepository<UserLockClaimEntity> userLockClaimRepo,
            IConfiguration configuration)
        {
            this.currentUserService = currentUserService;
            this.locationService = locationService;
            this.lockControlService = lockControlService;
            this.emailService = emailService;
            this.userService = userService;
            this.dateTimeService = dateTimeService;
            this.lockRepo = lockRepo;
            this.accessHistoryRepo = accessHistoryRepo;
            this.userLockClaimRepo = userLockClaimRepo;
            this.configuration = configuration;
        }

        public async Task AccessLock(AccessLockRequestDto requestDto)
        {
            var needConirm = false;
            //validation
            var validator = new LockAccessrRequestValidator();
            var valdationResult = validator.Validate(requestDto);
            valdationResult.Resolve();

            //check lock
            var thisLock = await CheckLockAsync(requestDto.DoorKeyCode);
            var userFromLockDistance = locationService.CalculateDistance(thisLock.Latitude, requestDto.Location.Latitude,
                       thisLock.Longitude, requestDto.Location.Longitude);

            //check distance
            if (userFromLockDistance > 10) // if 10 or less meters way from lock the user can open
                throw new ApplicationException("Too far to access the lock, the distance should be 10 or less meters");

            //check claim
            var lastLockClaim = await userLockClaimRepo.FirstByConditionAsync(x => x.LockId == thisLock.Id && x.UserId == currentUserService.UserId, AsNoTracking: false);
            if (lastLockClaim is null)
            {
                lastLockClaim = await userLockClaimRepo.InsertAsync(new UserLockClaimEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    ClaimType = currentUserService.UserType == UserTypeEnum.Guest ? ClaimTypeEnum.Once : ClaimTypeEnum.Always,
                    LockId = thisLock.Id,
                    UserId = currentUserService.UserId,
                    AccessUntil = currentUserService.UserType == UserTypeEnum.Guest ? dateTimeService.Now.AddSeconds(-1) : dateTimeService.Now.AddYears(1)
                });
                if (currentUserService.UserType == UserTypeEnum.Guest)
                    needConirm = true;
            }
            else if (lastLockClaim.AccessUntil < dateTimeService.Now)
            {
                if (currentUserService.UserType == UserTypeEnum.Guest)
                    needConirm = true;
            }
            // notify admin with user access
            var thisUser = await userService.GetByIdAsync(currentUserService.UserId);
            NotifyAdminToGrantAccess(thisUser.Email, lastLockClaim.Id, needConirm);

            await accessHistoryRepo.InsertAsync(new LockAccessHistoryEntity
            {
                AccessStatus = needConirm ? LockAccessStatusEnum.AccessDenied : LockAccessStatusEnum.AccessGranted,
                DoorLockId = thisLock.Id,
                Reason = "Accessing lock",
                UserId = thisUser.Id,
            });

            // save to db
            await userLockClaimRepo.SaveChangesAsync();
            await accessHistoryRepo.SaveChangesAsync();

            if (needConirm)
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
            var lockClaim = await userLockClaimRepo.GetAsync(claimId, false);
            if (lockClaim is null)
                throw new KeyNotFoundException("ClaimId is not valid");

            if (lockClaim.AccessUntil > dateTimeService.Now)
                throw new ApplicationException("Access already granded");
            //check lock
            var doorLock = await lockRepo.GetAsync(lockClaim.LockId);
            if (doorLock is null)
                throw new KeyNotFoundException("Lock does not exist");

            //update claim
            var user = await userService.GetByIdAsync(lockClaim.UserId);
            lockClaim.AccessUntil = user.UserType == UserTypeEnum.Guest ?
                dateTimeService.Now.AddHours(1) : dateTimeService.Now.AddYears(1);

            await userLockClaimRepo.SaveChangesAsync();

            // update history
            var history = await accessHistoryRepo.FirstByConditionAsync(x => x.UserId == lockClaim.UserId && x.DoorLockId == lockClaim.LockId, AsNoTracking: false);
            if (history is not null)
            {
                history.AccessStatus = LockAccessStatusEnum.AccessGranted;
                await accessHistoryRepo.SaveChangesAsync();
            }
            // open the door
            await lockControlService.OpenLock(doorLock.DoorKeyCode);
        }

        #region Private
        async Task<DoorLockEntity> CheckLockAsync(string doorKeyCode)
        {
            var thisLock = await lockRepo.FirstByConditionAsync(l => l.DoorKeyCode.Equals(doorKeyCode) && l.RecordStatus != RecordStatusEnum.Deleted);
            if (thisLock is null)
                throw new KeyNotFoundException("DoorKeyCode is not exist");
            if (thisLock.RecordStatus == RecordStatusEnum.InActive)
                throw new ApplicationException("DoorKeyCode is not active");
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
