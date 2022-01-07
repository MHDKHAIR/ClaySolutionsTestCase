using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Application.Common.Interfaces;
using Infrastructure.Common;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        readonly MailSettings _mailSettings;
        readonly IConfiguration _configuration;
        readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _mailSettings = new MailSettings();
            _configuration.GetSection("MailSettings").Bind(_mailSettings);
        }
        public void SendEmailTask(IMailRequest mailRequest)
        {
            Task.Run(() =>
              {
                  try
                  {
                      var email = new MimeMessage
                      {
                          Sender = MailboxAddress.Parse(_mailSettings.Mail),
                      };
                      email.Sender.Name = _mailSettings.DisplayName;
                      email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                      email.Subject = mailRequest.Subject;
                      var builder = new BodyBuilder();
                      builder.HtmlBody = mailRequest.Body;
                      email.Body = builder.ToMessageBody();
                      using var smtp = new SmtpClient();
                      smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                      smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                      smtp.Send(email);
                      smtp.Disconnect(true);
                      _logger.LogInformation($"Email send to: {mailRequest.ToEmail}");
                  }
                  catch (Exception e)
                  {
                      _logger.LogError(e.Message);
                  }
              });
        }
    }
}
