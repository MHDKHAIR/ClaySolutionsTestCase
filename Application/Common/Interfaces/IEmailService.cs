using Domain.Common;

namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        void SendEmailTask(MailRequest mailRequest);
    }
}
