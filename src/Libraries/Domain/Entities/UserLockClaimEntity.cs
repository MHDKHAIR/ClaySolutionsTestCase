using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class UserLockClaimEntity : BaseEntity<string>
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(DoorLock))]
        public Guid LockId { get; set; }

        public ClaimTypeEnum ClaimType { get; set; }

        public DateTime? AccessUntil { get; set; }

        public UserEntity User { get; set; }
        public DoorLockEntity DoorLock { get; set; }
    }
}
