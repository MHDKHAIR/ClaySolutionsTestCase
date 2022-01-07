using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class LockAccessHistoryEntity : BaseEntity<int>
    {
        public LockAccessStatusEnum AccessStatus { get; set; }
        public string Reason { get; set; }


        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public UserEntity User { get; set; }

        [ForeignKey(nameof(DoorLock))]
        public Guid DoorLockId { get; set; }
        public DoorLockEntity DoorLock { get; set; }
    }
}
