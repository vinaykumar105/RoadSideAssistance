using Ardalis.GuardClauses;
using RoadSideAssistance.Application.Contracts.Persistence;
using RoadSideAssistance.Application.Contracts.Services;
using RoadSideAssistance.Application.Exceptions;
using RoadSideAssistance.Application.GuardClauses;
using RoadSideAssistance.Domain.Entities;

namespace RoadSideAssistance.Application.Services
{
    public class RoadSideAssistanceService : IRoadsideAssistanceService
    {
        private readonly IRoadsideServiceProviderRepository _roadSideServiceProviderRepository;
        private readonly IGeolocationService _geolocationService;

        public RoadSideAssistanceService(
            IRoadsideServiceProviderRepository roadSideServiceProviderRepository,
            IGeolocationService geolocationService)
        {
            _roadSideServiceProviderRepository =
                Guard.Against.Null(roadSideServiceProviderRepository, nameof(roadSideServiceProviderRepository));
            _geolocationService =
                Guard.Against.Null(geolocationService, nameof(geolocationService));
        }

        public SortedSet<Assistant> FindNearestAssistants(Geolocation geolocation, int limit)
        {
            Guard.Against.Invalid(geolocation, nameof(geolocation));
            Guard.Against.Negative(limit, nameof(limit));

            var assistants = _roadSideServiceProviderRepository
                .GetNearestServiceProviders(geolocation, limit)
                .Select(p => new Assistant()
                {
                    AssistantId = p.ServiceProviderId,
                    DistanceFromCustomer = _geolocationService.GetDistance(geolocation, p.Location)
                });

            return new SortedSet<Assistant>(assistants);
        }

        public void ReleaseAssistant(Customer customer, Assistant assistant)
        {
            Guard.Against.Invalid(customer, nameof(customer));
            Guard.Against.Invalid(assistant, nameof(assistant));

            var serviceProvider = _roadSideServiceProviderRepository.GetRoadSideServiceProvider(assistant.AssistantId);
            if(serviceProvider == null)
            {
                throw new ServiceProviderNotFoundException(assistant.AssistantId);
            }

            if(!serviceProvider.CustomerId.HasValue || !serviceProvider.CustomerId.Equals(customer.CustomerId))
            {
                throw new CustomerNotAssignedToServiceProviderException(customer.CustomerId, assistant.AssistantId);
            }

            _roadSideServiceProviderRepository.ReleaseServiceProviderFromCustomer(customer.CustomerId,
                assistant.AssistantId);
        }

        public Assistant? ReserveAssistant(Customer customer, Geolocation customerLocation)
        {
            Guard.Against.Invalid(customer, nameof(customer));
            Guard.Against.Invalid(customerLocation, nameof(customerLocation));

            var serviceProviderIdForCustomer = _roadSideServiceProviderRepository
                .GetServiceProviderIdForCustomer(customer.CustomerId);
            if(serviceProviderIdForCustomer.HasValue)
            {
                throw new CustomerAlreadyAssignedToServiceProviderException(customer.CustomerId, serviceProviderIdForCustomer.Value);
            }

            var assistants = FindNearestAssistants(customerLocation, 1);
            if(assistants.Count == 0)
            {
                return null;
            }

            var assistant = assistants.First();
            _roadSideServiceProviderRepository.AssignServiceProviderToCustomer(
                customer.CustomerId,
                assistant.AssistantId);

            return assistant;
        }

        public void UpdateAssistantLocation(Assistant assistant, Geolocation assistantLocation)
        {
            var serviceProvider = _roadSideServiceProviderRepository.GetRoadSideServiceProvider(assistant.AssistantId);
            if (serviceProvider == null)
            {
                throw new ServiceProviderNotFoundException(assistant.AssistantId);
            }

            _roadSideServiceProviderRepository.UpdateServiceProviderLocation(
                assistant.AssistantId,
                assistantLocation);
        }
    }
}
