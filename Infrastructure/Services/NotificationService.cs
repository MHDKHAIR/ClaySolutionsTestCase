using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        readonly IConfiguration _configuration;
        readonly IEmailService _emailService;
        readonly ICurrentUserService _currentUserService;

        public NotificationService(IConfiguration configuration, IEmailService emailService,
            ICurrentUserService currentUserService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _currentUserService = currentUserService;
        }
        public bool AccountConfirmationNotify(UserEntity user, string confirmationToken)
        {
            try
            {
                var cofirmationLink = _configuration.GetValue<string>("HostDomain");
                cofirmationLink = $"{cofirmationLink}api/account/confirmation?id={user.Id}&confirmationtoken={HttpUtility.UrlEncode(confirmationToken)}";
                _emailService.SendEmailTask(new MailRequest
                {
                    Subject = "Welcome to the matrix!",
                    ToEmail = user.Email,
                    Body = $"Your confirmation link:</br>{cofirmationLink}"
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool AccountActivationNotify(UserEntity user)
        {
            try
            {
                var cofirmationLink = _configuration.GetValue<string>("HostDomain");
                cofirmationLink = $"{cofirmationLink}api/admin/account_activation?UserId={user.Id}";
                _emailService.SendEmailTask(new MailRequest
                {
                    Subject = "Please confirm user account as employee!",
                    ToEmail = _configuration.GetValue<string>("AdminEmail"),
                    Body = $"Employee email: {user.Email}</br>Confirm user account link:</br>{cofirmationLink}"
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool GrantAccessNotify(UserEntity user, string claimId, bool needConfirm)
        {
            try
            {
                var withLink = string.Empty;
                if (_currentUserService.UserType == UserTypeEnum.Guest && needConfirm)
                {
                    var cofirmationLink = _configuration.GetValue<string>("HostDomain");
                    cofirmationLink += $"api/admin/grant_access?ClaimId={claimId}";
                    withLink = $"</br>Confirm user account link:</br>{cofirmationLink}";
                }
                _emailService.SendEmailTask(new MailRequest
                {
                    Subject = "User requested door access!",
                    ToEmail = _configuration.GetValue<string>("AdminEmail"),
                    Body = $"{_currentUserService.UserType} User email: {user.Email} requested access {withLink}"
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
