using System;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class BaseEntity<TKey> : IBaseEntity
    {
        public TKey Id { get; set; }
        public RecordStatusEnum RecordStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
    }
}
