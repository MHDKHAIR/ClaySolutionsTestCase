using Application.Common.Interfaces;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string UserId { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
