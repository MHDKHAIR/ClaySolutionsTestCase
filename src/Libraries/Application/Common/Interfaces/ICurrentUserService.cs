using Domain.Enums;

namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
