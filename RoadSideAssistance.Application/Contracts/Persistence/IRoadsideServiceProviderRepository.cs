using RoadSideAssistance.Domain.Entities;

namespace RoadSideAssistance.Application.Contracts.Persistence
{
    public interface IRoadsideServiceProviderRepository
    {
        RoadSideServiceProvider? GetRoadSideServiceProvider(Guid serviceProviderId);

        void UpdateServiceProviderLocation(Guid serviceProviderId, Geolocation location);

        void AssignServiceProviderToCustomer(Guid customerId, Guid serviceProviderId);

        void ReleaseServiceProviderFromCustomer(Guid customerId, Guid serviceProviderId);

        IEnumerable<RoadSideServiceProvider> GetNearestServiceProviders(Geolocation location, int limit);

        Guid? GetServiceProviderIdForCustomer(Guid customerId);
    }
}
