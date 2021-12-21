using Domain.Enums;

namespace Domain.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
