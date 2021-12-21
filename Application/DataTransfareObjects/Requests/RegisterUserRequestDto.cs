using Domain.Enums;

namespace Application.DataTransfareObjects.Requests
{
    public class RegisterUserRequestDto
    {
        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public UserTypeEnum UserType { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
