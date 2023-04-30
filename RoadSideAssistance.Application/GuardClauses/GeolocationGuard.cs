using Ardalis.GuardClauses;
using RoadSideAssistance.Domain.Entities;

namespace RoadSideAssistance.Application.GuardClauses
{
    public static class GeolocationGuard
    {
        public static void Invalid(this IGuardClause guardClause, Geolocation input, string parameterName)
        {
            // TODO Implementation   
        }
    }
}
