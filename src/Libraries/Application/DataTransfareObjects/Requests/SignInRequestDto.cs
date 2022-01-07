namespace Application.DataTransfareObjects.Requests
{
    public class SignInRequestDto
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

}
