using Domain.Enums;
using Domain.Interfaces.Services;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string UserId { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
