using System;
using Domain.Enums;

namespace Application.DataTransfareObjects.Responses
{
    public class UserAccessHistortLocksResponseDto
    {
        public LockAccessStatusEnum AccessStatus { get; set; }
        public string Reason { get; set; }
        public string DoorName { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int FloorIndex { get; set; }
        public string BuildingName { get; set; }
    }
}
