using System;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class UserEntity : IdentityUser, IBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserTypeEnum UserType { get; set; }
        public RecordStatusEnum RecordStatus { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
    }
}