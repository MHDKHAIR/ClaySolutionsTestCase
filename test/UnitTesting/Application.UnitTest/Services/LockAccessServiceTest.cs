using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;
using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTest.Services
{
    public class LockAccessServiceTest
    {
        readonly Mock<ICurrentUserService> _currentUserService;
        readonly Mock<IGeoLocationService> _geoLocationService;
        readonly Mock<ILockControlService> _lockControlService;
        readonly Mock<IUserService> _userService;
        readonly Mock<IDateTimeService> _dateTimeService;
        readonly Mock<IEfRepository<DoorLockEntity>> _lockRepo;
        readonly Mock<IEfRepository<LockAccessHistoryEntity>> _accessHistoryRepo;
        readonly Mock<IEfRepository<UserLockClaimEntity>> _userLockClaimRepo;
        readonly Mock<INotificationService> _notificationService;
        readonly Mock<ILogger<ILockControlService>> _lockControlServerLogger;
        readonly LockAccessService _lockAccessService;
        public LockAccessServiceTest()
        {
            _currentUserService = new Mock<ICurrentUserService>();
            _geoLocationService = new Mock<IGeoLocationService>();
            _lockControlService = new Mock<ILockControlService>();
            _userService = new Mock<IUserService>();
            _dateTimeService = new Mock<IDateTimeService>();
            _lockRepo = new Mock<IEfRepository<DoorLockEntity>>();
            _accessHistoryRepo = new Mock<IEfRepository<LockAccessHistoryEntity>>();
            _userLockClaimRepo = new Mock<IEfRepository<UserLockClaimEntity>>();
            _notificationService = new Mock<INotificationService>();
            _lockControlServerLogger = new Mock<ILogger<ILockControlService>>();

            _lockAccessService = new LockAccessService(
                _currentUserService.Object, _geoLocationService.Object,
                _lockControlService.Object, _userService.Object,
                _dateTimeService.Object, _lockRepo.Object,
                _accessHistoryRepo.Object, _userLockClaimRepo.Object, _notificationService.Object);
        }

        [Fact]
        public async Task AccessLockShouldReturnValidAccessDataWhenUserCanClaimToAccessDoorLock()
        {
            // Arrenge
            var requestDto = new AccessLockRequestDto
            {
                DoorKeyCode = "0R39HU0C",
                Location = new AccessLocationDto
                {
                    Longitude = 52.35660693512942,
                    Latitude = 4.8397535190171945
                }
            };
            var lockEntity = new DoorLockEntity
            {
                Id = Guid.NewGuid(),
                RecordStatus = Domain.Enums.RecordStatusEnum.Active,
                DoorKeyCode = requestDto.DoorKeyCode,
                Latitude = requestDto.Location.Latitude,
                Longitude = requestDto.Location.Longitude,
            };
            var userId = Guid.NewGuid().ToString();
            var claimEntity = new UserLockClaimEntity
            {
                Id = Guid.NewGuid().ToString(),
                LockId = lockEntity.Id,
                UserId = userId,
            };
            _lockRepo.Setup(x => x.FirstByConditionAsync(l => l.DoorKeyCode.Equals(lockEntity.DoorKeyCode) && l.RecordStatus != RecordStatusEnum.Deleted, true))
                .ReturnsAsync(lockEntity);
            _currentUserService.Setup(x => x.UserId).Returns(userId);
            _currentUserService.Setup(x => x.UserType).Returns(UserTypeEnum.Employee);
            _userLockClaimRepo.Setup(x => x.FirstByConditionAsync(x => x.LockId == lockEntity.Id && x.UserId == userId, false))
                .ReturnsAsync(claimEntity);
            // Act
            var accessResult = await _lockAccessService.AccessLock(requestDto);
            // Assert
            Assert.NotNull(accessResult);
            Assert.Equal(requestDto.DoorKeyCode, requestDto.DoorKeyCode);
        }
    }
}
