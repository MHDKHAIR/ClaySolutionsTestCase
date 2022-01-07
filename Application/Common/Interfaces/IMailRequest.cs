namespace Application.Common.Interfaces
{
    public interface IMailRequest
    {
         string ToEmail { get; set; }
         string Subject { get; set; }
         string Body { get; set; }
    }
}
