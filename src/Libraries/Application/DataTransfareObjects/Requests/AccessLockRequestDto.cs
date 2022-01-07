
namespace Application.DataTransfareObjects.Requests
{
    public class AccessLockRequestDto
    {
        /// <summary>
        /// This is an 8 characters string
        /// </summary>
        public string DoorKeyCode { get; set; }
        public AccessLocationDto Location { get; set; }
    }
    public class AccessLocationDto
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
