using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Application.DataTransfareObjects.Requests;
using Application.DataTransfareObjects.Responses;
using Application.Services.Contracts;
using Application.Utils;
using Application.Validators;
using Application.Extentions;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Services;
using System.Linq;
using System.Web;

namespace Application.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<UserEntity> _userManager;
        readonly SignInManager<UserEntity> _signInManager;
        readonly JwtUtils _jwtUtils;
        readonly ICurrentUserService _currentUserService;
        readonly IDateTimeService _dateTimeService;
        readonly IEmailService _emailService;
        readonly IConfiguration _configuration;

        public UserService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager,
            JwtUtils jwtUtils, ICurrentUserService currentUserService,
            IDateTimeService dateTimeService, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtUtils = jwtUtils;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _emailService = emailService;
            _configuration = configuration;
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

            // save changes to db
            //await _signInManager.SignInAsync(user, dto.RememberMe);

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
                // Employee user need the admin to activate it
                // Guest no need but he can't access a lock without admin permission
                RecordStatus = dto.UserType == UserTypeEnum.Employee ? RecordStatusEnum.InActive : RecordStatusEnum.Active,
                EmailConfirmed = false,
                PhoneNumberConfirmed = true,
                CreatedAt = _dateTimeService.Now,
                CreatedBy = "System"
            };
            // save changes to db
            ResolveIdentityResult(await _userManager.CreateAsync(newUser));

            //email user
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            SendConfirmationEmail(newUser.Id, dto.UserEmail, confirmationToken);
            // email admin for demo purpose
            if (newUser.UserType == UserTypeEnum.Employee)
                SendToAdminEmail(newUser);

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
            if (string.IsNullOrEmpty(userId))
                throw new Exception("UserId is empty");

            var user = await _userManager.FindByIdAsync(userId);
            if (user.RecordStatus is RecordStatusEnum.Active)
                throw new ApplicationException("User already activated");
            if (user.UserType is not UserTypeEnum.Employee)
                throw new ApplicationException("Only Employee allowed to be activated");

            user.RecordStatus = RecordStatusEnum.Active;
            ResolveIdentityResult(await _userManager.UpdateAsync(user));
        }
        public async Task EmailConfirmationAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new ApplicationException("User not found");
            if (await _userManager.IsEmailConfirmedAsync(user))
                throw new ApplicationException("Email is confirmed already");
            ResolveIdentityResult(await _userManager.ConfirmEmailAsync(user, token));
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
            ResolveIdentityResult(await _userManager.UpdateAsync(user));
        }

        // private
        void SendConfirmationEmail(string userId, string toEmail, string confirmationToken)
        {
            var cofirmationLink = _configuration.GetValue<string>("HostDomain");
            cofirmationLink += $"api/account/confirmation?id={userId}&confirmationtoken={HttpUtility.UrlEncode(confirmationToken)}";
            _emailService.SendEmailTask(new MailRequest
            {
                Subject = "Welcome to the matrix!",
                ToEmail = toEmail,
                Body = $"Your confirmation link:</br>{cofirmationLink}"
            });
        }
        // for demo purpose
        void SendToAdminEmail(UserEntity user)
        {
            var cofirmationLink = _configuration.GetValue<string>("HostDomain");
            cofirmationLink += $"api/admin/account_activation?UserId={user.Id}";
            _emailService.SendEmailTask(new MailRequest
            {
                Subject = "Please confirm user account as employee!",
                ToEmail = _configuration.GetValue<string>("AdminEmail"),
                Body = $"Employee email: {user.Email}</br>Confirm user account link:</br>{cofirmationLink}"
            });
        }
        void ResolveIdentityResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                result.Errors.ToList().ForEach(e => errors += e.Description + Environment.NewLine);
                throw new ApplicationException(errors);
            }
        }
    }
}
