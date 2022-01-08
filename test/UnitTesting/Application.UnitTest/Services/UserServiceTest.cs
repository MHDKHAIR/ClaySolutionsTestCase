using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;
using Application.Services;
using Application.Utils;
using Castle.Core.Resource;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Application.UnitTest.Services
{
    public class UserServiceTest
    {
        readonly Mock<UserManager<UserEntity>> _userManager;
        readonly Mock<IJwtUtils> _jwtUtils;
        readonly Mock<ICurrentUserService> _currentUserService;
        readonly Mock<IDateTimeService> _dateTimeService;
        readonly Mock<INotificationService> _notificationService;
        readonly UserService _service;

        public UserServiceTest()
        {
            var testUser = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Email = "clay@admin.com",
                UserName = "clay@admin.com",
                PasswordHash = "AQAAAAEAACcQAAAAED1aSLNoy9ienwSIvL1r8kmQvvYQTgKibYCvuxEfB3gYO5dbkoTi1RKjtZ5RHBA6HQ==",
                FirstName = "admin",
                LastName = "admin",
                CreatedAt = DateTime.Now,
                EmailConfirmed = true,
                UserType = Domain.Enums.UserTypeEnum.Admin,
            };
            _userManager = MockUserManager(new List<UserEntity>
            {
                testUser
            });
            _jwtUtils = new Mock<IJwtUtils>();
            _currentUserService = new Mock<ICurrentUserService>();
            _dateTimeService = new Mock<IDateTimeService>(); ;
            _notificationService = new Mock<INotificationService>();
            _service = new UserService(_userManager.Object, _jwtUtils.Object, _currentUserService.Object,
                 _dateTimeService.Object, _notificationService.Object);
        }

        [Fact]
        public async Task GetUserByIdShouldReturnUserWhenUserExist()
        {
            // Arrenge
            var userId = Guid.NewGuid().ToString();
            var userEmail = "clay@admin.com";
            _userManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new UserEntity
                {
                    Id = userId,
                    Email = "clay@admin.com",
                    UserName = "clay@admin.com",
                    FirstName = "admin",
                    LastName = "admin",
                    CreatedAt = DateTime.Now,
                    EmailConfirmed = true,
                    UserType = Domain.Enums.UserTypeEnum.Admin,
                });
            // Act
            var user = await _service.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(userId, user.Id);
            Assert.Equal(userEmail, user.Email);
        }
        [Fact]
        public async Task GetUserByIdShouldThrowApplicationExceptionWhenIdIsEmpty()
        {
            // Arrenge

            // Act
            var caughtException = await Assert.ThrowsAsync<Common.Exeptions.ApplicationException>(
                () => _service.GetByIdAsync(string.Empty));

            // Assert
            Assert.Equal("id must have a value", caughtException.Message);
        }
        [Fact]
        public async Task GetUserByIdShouldThrowNotFoundExceptionWhenUserDoesNotExist()
        {
            // Arrenge
            var userId = Guid.NewGuid().ToString();
            var otherUserId = Guid.NewGuid().ToString();
            _userManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new UserEntity
                {
                    Id = userId,
                    Email = "clay@admin.com",
                    UserName = "clay@admin.com",
                    FirstName = "admin",
                    LastName = "admin",
                    CreatedAt = DateTime.Now,
                    EmailConfirmed = true,
                    UserType = Domain.Enums.UserTypeEnum.Admin,
                });

            // Act
            var caughtException = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetByIdAsync(otherUserId));

            // Assert
            Assert.Equal("User not found", caughtException.Message);
        }
        [Fact]
        public async Task AuthenticateShouldReturnSignInResponseWithTokenWhenUserExist()
        {
            // Arrenge
            var userId = Guid.NewGuid().ToString();
            var userEmail = "clay@admin.com";
            var user = new UserEntity
            {
                Id = userId,
                Email = userEmail,
                UserName = userEmail,
                PasswordHash = "AQAAAAEAACcQAAAAED1aSLNoy9ienwSIvL1r8kmQvvYQTgKibYCvuxEfB3gYO5dbkoTi1RKjtZ5RHBA6HQ==",
                CreatedAt = DateTime.Now,
                EmailConfirmed = true,
                UserType = Domain.Enums.UserTypeEnum.Admin,
            };
            _userManager.Setup(x => x.FindByEmailAsync(userEmail))
                .ReturnsAsync(user);
            _jwtUtils.Setup(x => x.GenerateJwtToken(user, false))
                .Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImRjYWRhOGEwLWM1MTAtNGRiNS1iYTM3LTE1YTY4ODQxNWJmMiIsInVzZXJ0eXBlIjoiQWRtaW4iLCJuYmYiOjE2NDE1NTUwODEsImV4cCI6MTY0MTU1ODY4MSwiaWF0IjoxNjQxNTU1MDgxfQ.BJ70UKtM7_QU0wwoal8H5hd_RsHfQtbI4e7q01KONWo");
            var signInDtp = new SignInRequestDto
            {
                Password = "SuperAdmin@2022",
                RememberMe = false,
                UserEmail = userEmail
            };
            // Act
            var signInResponseDto = await _service.AuthenticateAsync(signInDtp);
            // Assert
            Assert.NotNull(signInResponseDto);
            Assert.NotNull(signInResponseDto.JwtToken);
        }
        [Fact]
        public async Task AuthenticateShouldReturnApplicationExceptionWhenEmailsWrong()
        {
            // Arrenge
            var userId = Guid.NewGuid().ToString();
            var userEmail = "clay@admin.com";
            var user = new UserEntity
            {
                Id = userId,
                Email = userEmail,
                UserName = userEmail,
                PasswordHash = "AQAAAAEAACcQAAAAED1aSLNoy9ienwSIvL1r8kmQvvYQTgKibYCvuxEfB3gYO5dbkoTi1RKjtZ5RHBA6HQ==",
                CreatedAt = DateTime.Now,
                EmailConfirmed = true,
                UserType = Domain.Enums.UserTypeEnum.Admin,
            };
            _userManager.Setup(x => x.FindByEmailAsync(userEmail))
                .ReturnsAsync(user);
            var signInDtp = new SignInRequestDto
            {
                Password = "Supern@2022",
                RememberMe = false,
                UserEmail = "cla1y@admin.com"
            };
            // Act
            var caughtException = await Assert.ThrowsAsync<Common.Exeptions.ApplicationException>(
                    async () => await _service.AuthenticateAsync(signInDtp));
            // Assert
            Assert.Equal("UserEmail or Password is incorrect", caughtException.Message);
        }
        [Fact]
        public async Task AuthenticateShouldReturnApplicationExceptionWhenPasswordIsWrong()
        {
            // Arrenge
            var userId = Guid.NewGuid().ToString();
            var userEmail = "clay@admin.com";
            var user = new UserEntity
            {
                Id = userId,
                Email = userEmail,
                UserName = userEmail,
                PasswordHash = "AQAAAAEAACcQAAAAED1aSLNoy9ienwSIvL1r8kmQvvYQTgKibYCvuxEfB3gYO5dbkoTi1RKjtZ5RHBA6HQ==",
                CreatedAt = DateTime.Now,
                EmailConfirmed = true,
                UserType = Domain.Enums.UserTypeEnum.Admin,
            };
            _userManager.Setup(x => x.FindByEmailAsync(userEmail))
                .ReturnsAsync(user);
            var signInDtp = new SignInRequestDto
            {
                Password = "Supern@2022",
                RememberMe = false,
                UserEmail = userEmail
            };
            // Act
            var caughtException = await Assert.ThrowsAsync<Common.Exeptions.ApplicationException>(
                    async () => await _service.AuthenticateAsync(signInDtp));
            // Assert
            Assert.Equal("UserEmail or Password is incorrect", caughtException.Message);
        }


        private Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
    }
}
