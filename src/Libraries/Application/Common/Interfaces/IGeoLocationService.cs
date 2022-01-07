
namespace Application.Common.Interfaces
{
    public interface IGeoLocationService
    {
        double CalculateDistance(double lat1,
                            double lat2,
                            double lon1,
                            double lon2);
    }
}
