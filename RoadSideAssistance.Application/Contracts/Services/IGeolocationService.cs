using RoadSideAssistance.Domain.Entities;

namespace RoadSideAssistance.Application.Contracts.Services
{
    public interface IGeolocationService
    {
        double GetDistance(Geolocation locationA, Geolocation locationB);
    }
}
