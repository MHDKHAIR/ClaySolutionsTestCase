using Domain.Common;

namespace Domain.Interfaces.Services
{
    public interface IEmailService
    {
        void SendEmailTask(MailRequest mailRequest);
    }
}
