namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        void SendEmailTask(IMailRequest mailRequest);
    }
}
