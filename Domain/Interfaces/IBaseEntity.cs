using System;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IBaseEntity
    {
        RecordStatusEnum RecordStatus { get; set; }
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
        string ModifiedBy { get; set; }
    }
}
