
using NetTopologySuite.Geometries;
using RoadSideAssistance.Application.Contracts.Services;
using RoadSideAssistance.Domain.Entities;

namespace RoadSideAssistance.Application.Services
{
    public class GeolocationService : IGeolocationService
    {
        public double GetDistance(Geolocation locationA, Geolocation locationB)
        {
            return new Point(locationA.X, locationA.Y) { SRID = 4326 }
                .Distance(new Point(locationB.X, locationB.Y) { SRID = 4326 });
        }
    }
}
