using System;

namespace Application.DataTransfareObjects.Responses
{
    public record AccessLockResponseDto
    {
        public string DoorKeyCode { get; set; }
        public string ClaimType { get; set; }
        public DateTime AccessValidUntil { get; set; }
    }
}
