using System;

namespace Application.DataTransfareObjects.Responses
{
    public record SignInResponseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
