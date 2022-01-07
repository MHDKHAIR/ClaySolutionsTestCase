using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;
using Application.Utils;
using Application.Validators;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using Application.Services.Interfaces;
using Application.Common.Interfaces;

namespace Application.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<UserEntity> _userManager;
        readonly SignInManager<UserEntity> _signInManager;
        readonly IJwtUtils _jwtUtils;
        readonly ICurrentUserService _currentUserService;
        readonly IDateTimeService _dateTimeService;
        readonly INotificationService _notificationService;

        public UserService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager,
            IJwtUtils jwtUtils, ICurrentUserService currentUserService,
            IDateTimeService dateTimeService, INotificationService notificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtUtils = jwtUtils;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _notificationService = notificationService;
        }

        public async Task<UserEntity> GetByIdAsync(string id, params string[] include)
        {
            var userEntity = await _userManager.FindByIdAsync(id);

            if (userEntity is null)
                throw new KeyNotFoundException("User not found");

            return userEntity;
        }
        public async Task<SignInResponseDto> AuthenticateAsync(SignInRequestDto dto)
        {
            // validate
            var validator = new SignInRequestValidator();
            var validationResult = await validator.ValidateAsync(dto);
            validationResult.Resolve();

            //check
            var user = await _userManager.FindByEmailAsync(dto.UserEmail);
            if (user is null)
                throw new ApplicationException("UserEmail or Password is incorrect");

            if (user.RecordStatus is RecordStatusEnum.Deleted)
                throw new ApplicationException("User does not exist");

            if (user.RecordStatus is RecordStatusEnum.InActive)
                throw new ApplicationException("User is not active");

            //password
            PasswordHasher<UserEntity> hasher = new PasswordHasher<UserEntity>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result is PasswordVerificationResult.Failed)
                throw new ApplicationException("UserEmail or Password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            var canSignIn = await _signInManager.CanSignInAsync(user);

            return new SignInResponseDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Email,
                JwtToken = jwtToken,
                IsEmailConfirmed = canSignIn
            };
        }
        public async Task<RegisterUserResponseDto> RegisterUserAsync(RegisterUserRequestDto dto)
        {
            // validate
            var validator = new RegistrationRequestValidator();
            var validationResult = await validator.ValidateAsync(dto);
            validationResult.Resolve();
            //check
            var user = await _userManager.FindByEmailAsync(dto.UserEmail);
            if (user is not null)
                throw new ApplicationException("UserEmail is already exist");
            //password
            var hashedPassword = new PasswordHasher<UserEntity>().HashPassword(user, dto.Password);
            var userId = Guid.NewGuid().ToString();
            var newUser = new UserEntity
            {
                Id = userId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.UserEmail,
                UserName = dto.UserEmail,
                NormalizedEmail = dto.UserEmail.ToUpper(),
                NormalizedUserName = dto.UserEmail.ToUpper(),
                PasswordHash = hashedPassword,
                UserType = dto.UserType,
                RecordStatus = dto.UserType == UserTypeEnum.Employee ? RecordStatusEnum.InActive : RecordStatusEnum.Active,
                EmailConfirmed = false,
                PhoneNumberConfirmed = true,
                CreatedAt = _dateTimeService.Now,
                CreatedBy = "System"
            };
            // save changes to db
            (await _userManager.CreateAsync(newUser))
                .ResolveIdentityErrorResult();
            //email user
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            _notificationService.AccountConfirmationNotify(newUser, confirmationToken);
            // email admin for demo purpose
            if (newUser.UserType == UserTypeEnum.Employee)
                _notificationService.AccountActivationNotify(newUser);
            return new RegisterUserResponseDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserEmail = dto.UserEmail,
                UserType = dto.UserType,
                Message = "Thank you for joining" + Environment.NewLine + "Please confrim your email"
            };
        }
        public async Task ActivateAccountAsync(string userId)
        {
            if (_currentUserService.UserType is Domain.Enums.UserTypeEnum.Admin)
                throw new UnauthorizedAccessException("This is only for admin");

            if (string.IsNullOrEmpty(userId))
                throw new Exception("UserId is empty");

            var user = await _userManager.FindByIdAsync(userId);
            if (user.RecordStatus is RecordStatusEnum.Active)
                throw new ApplicationException("User already activated");
            if (user.UserType is not UserTypeEnum.Employee)
                throw new ApplicationException("Only Employee allowed to be activated");

            user.RecordStatus = RecordStatusEnum.Active;
            (await _userManager.UpdateAsync(user))
                .ResolveIdentityErrorResult();
        }
        public async Task EmailConfirmationAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new ApplicationException("User not found");
            if (await _userManager.IsEmailConfirmedAsync(user))
                throw new ApplicationException("Email is confirmed already");
            (await _userManager.ConfirmEmailAsync(user, token))
                .ResolveIdentityErrorResult();
        }
        public async Task DeleteAccountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                userId = _currentUserService?.UserId ?? string.Empty;
            if (string.IsNullOrEmpty(userId))
                throw new ApplicationException("UserId is empty");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || user.RecordStatus == RecordStatusEnum.Deleted)
                throw new ApplicationException("User not found");

            user.RecordStatus = RecordStatusEnum.Deleted;
            (await _userManager.UpdateAsync(user))
                .ResolveIdentityErrorResult();
        }
    }
}
