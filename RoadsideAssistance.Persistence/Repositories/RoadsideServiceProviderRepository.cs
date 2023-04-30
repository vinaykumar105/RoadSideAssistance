using Ardalis.GuardClauses;
using RoadSideAssistance.Application.Contracts.Persistence;
using RoadSideAssistance.Application.Contracts.Services;
using RoadSideAssistance.Domain.Entities;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace RoadsideAssistance.Persistence.Repositories
{
    public class RoadsideServiceProviderRepository: IRoadsideServiceProviderRepository
    {
        private readonly ImmutableDictionary<Guid, RoadSideServiceProvider> _providers;
        private readonly IGeolocationService _geoLocationService;

        public RoadsideServiceProviderRepository(
            IEnumerable<RoadSideServiceProvider> providers,
            IGeolocationService geoLocationService)
        {
            Guard.Against.Null(providers, nameof(providers));
            this._geoLocationService = Guard.Against.Null(geoLocationService, nameof(geoLocationService));

            this._providers = new ConcurrentDictionary<Guid, RoadSideServiceProvider>(providers.ToDictionary(p => p.ServiceProviderId))
                .ToImmutableDictionary();
        }

        public void AssignServiceProviderToCustomer(Guid customerId, Guid serviceProviderId)
        {
            _providers[serviceProviderId].CustomerId = customerId;
        }

        public Guid? GetServiceProviderIdForCustomer(Guid customerId)
        {
            return _providers.Values.FirstOrDefault(p => p.CustomerId == customerId)?.ServiceProviderId;
        }

        public IEnumerable<RoadSideServiceProvider> GetNearestServiceProviders(Geolocation location, int limit)
        {
            return _providers
                .Select(p => new {
                    Distance = this._geoLocationService.GetDistance(location, p.Value.Location),
                    Provider = p.Value
                })
                .OrderBy(c => c.Distance)
                .Take(limit)
                .Select(c => c.Provider);
        }

        public RoadSideServiceProvider? GetRoadSideServiceProvider(Guid serviceProviderId)
        {
            _providers.TryGetValue(serviceProviderId, out var provider);
            return provider;
        }

        public void ReleaseServiceProviderFromCustomer(Guid customerId, Guid serviceProviderId)
        {
            _providers[serviceProviderId].CustomerId = null;
        }

        public void UpdateServiceProviderLocation(Guid serviceProviderId, Geolocation location)
        {
            _providers[serviceProviderId].Location = location;
        }
    }
}
