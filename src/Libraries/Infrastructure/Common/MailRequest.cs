using Application.Common.Interfaces;

namespace Infrastructure.Common
{
    public record MailRequest : IMailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
