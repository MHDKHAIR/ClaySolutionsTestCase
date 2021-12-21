using System;

namespace Application.DataTransfareObjects.Responses
{
    public class UserLocksResponseDto
    {
        public string DoorKeyCode { get; set; }
        public string DoorName { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int FloorIndex { get; set; }
        public string BuildingName { get; set; }
    }
}
