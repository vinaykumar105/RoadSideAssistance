using FluentAssertions;
using RoadsideAssistance.Persistence.Repositories;
using RoadSideAssistance.Application.Contracts.Persistence;
using RoadSideAssistance.Application.Contracts.Services;
using RoadSideAssistance.Application.Exceptions;
using RoadSideAssistance.Application.Services;
using RoadSideAssistance.Domain.Entities;
using Xunit;

namespace RoadsideAssistance.Application.UnitTests.Services
{
    public class RoadSideAssistanceServiceTests
    {
        private readonly RoadSideServiceProvider[] _providers;
        private readonly IRoadsideServiceProviderRepository _repository;
        private readonly Geolocation _laLocation = new Geolocation { Y = 34.0522, X = 118.2437 };
        private IRoadsideAssistanceService _service;

        public RoadSideAssistanceServiceTests()
        {
            _providers = new RoadSideServiceProvider[]
            {
                new RoadSideServiceProvider
                { 
                    ServiceProviderId = Guid.NewGuid(),
                    CustomerId = null,
                    Location = new Geolocation() { X = 122.3321, Y = 47.6062} // Seattle
                },
                new RoadSideServiceProvider
                {
                    ServiceProviderId = Guid.NewGuid(),
                    CustomerId = null,
                    Location = new Geolocation() { X = 123.1207, Y = 49.2827} // Vancouver
                },
                new RoadSideServiceProvider
                {
                    ServiceProviderId = Guid.NewGuid(),
                    CustomerId = null,
                    Location = new Geolocation() { X = 87.6298, Y = 41.8781} // Chicago
                },
                new RoadSideServiceProvider
                {
                    ServiceProviderId = Guid.NewGuid(),
                    CustomerId = null,
                    Location = new Geolocation() { X = 122.6784, Y = 45.5152} // Portland
                },
            };
            _repository = new RoadsideServiceProviderRepository(_providers, new GeolocationService());

            _service = new RoadSideAssistanceService(_repository, new GeolocationService());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ShouldUpdateAssistantLocation(int repositoryIndexToModify)
        {
            var targetProvider = _providers[repositoryIndexToModify];
            _service.UpdateAssistantLocation(
                new Assistant { AssistantId = targetProvider.ServiceProviderId },
                _laLocation);

            targetProvider.Location.Should().Be(_laLocation);
        }

        [Fact]
        public void ShouldThrowWhenUpdateAssistantLocationReceivesMissingAssistantId()
        {
            var method = () => _service.UpdateAssistantLocation(
            new Assistant { AssistantId = Guid.NewGuid()},
            _laLocation);

            method.Should().Throw<ServiceProviderNotFoundException>();
        }

        [Fact]
        public void ShouldReturnSortedSetOfAssistants()
        {
            var assistantsList = _service.FindNearestAssistants(_laLocation, 10).ToList(); // From LA
            assistantsList[0].AssistantId.Should().Be(_providers[3].ServiceProviderId); // Portland
            assistantsList[1].AssistantId.Should().Be(_providers[0].ServiceProviderId); // Seattle
            assistantsList[2].AssistantId.Should().Be(_providers[1].ServiceProviderId); // Vancouver
            assistantsList[3].AssistantId.Should().Be(_providers[2].ServiceProviderId); // Chicago
        }

        [Fact]
        public void ShouldReturnLimitedSortedSetOfAssistants()
        {
            var assistantsList = _service.FindNearestAssistants(_laLocation, 2).ToList();
            assistantsList.Count.Should().Be(2);
            assistantsList[0].AssistantId.Should().Be(_providers[3].ServiceProviderId);
            assistantsList[1].AssistantId.Should().Be(_providers[0].ServiceProviderId);
        }

        [Fact]
        public void ShouldReturnEmptySortedSetOfAssistantsWhenEmpty()
        {
            _service = new RoadSideAssistanceService(
                new RoadsideServiceProviderRepository(new RoadSideServiceProvider[] { }, new GeolocationService()),
                new GeolocationService());
            _service.FindNearestAssistants(_laLocation, 2).Should().BeEmpty();
        }

        [Fact]
        public void ShouldReserveAssistant()
        {
            var customerLocation = new Geolocation() { Y = 50.1162, X = 122.9535 };
            var customer = new Customer() { CustomerId = Guid.NewGuid() };
            var assistant = _service.ReserveAssistant(customer, customerLocation);
            assistant.Should().NotBeNull();
            assistant?.AssistantId.Should().Be(_providers[1].ServiceProviderId); // Vancouver                
            _providers[1].CustomerId.Should().Be(customer.CustomerId);
        }

        [Fact]
        public void ShouldThrowWhenCustomerIsAlreadyAssigned()
        {
            var customerLocation = new Geolocation() { Y = 50.1162, X = 122.9535 };
            var customer = new Customer() { CustomerId = Guid.NewGuid() };
            var assistant = _service.ReserveAssistant(customer, customerLocation);
            assistant?.AssistantId.Should().Be(_providers[1].ServiceProviderId); // Vancouver                
            _providers[1].CustomerId.Should().Be(customer.CustomerId);

            var method = () => _service.ReserveAssistant(customer, customerLocation);
            method.Should().Throw<CustomerAlreadyAssignedToServiceProviderException>();
        }

        [Fact]
        public void ShouldReseveNoAssistantIfProvidersAreEmpty()
        {
            _service = new RoadSideAssistanceService(
                new RoadsideServiceProviderRepository(new RoadSideServiceProvider[] { }, new GeolocationService()),
                new GeolocationService());
            var customerLocation = new Geolocation() { Y = 50.1162, X = 122.9535 };
            var assistant = _service.ReserveAssistant(new Customer() { CustomerId = Guid.NewGuid() }, customerLocation);
            assistant.Should().BeNull();                
        }

        [Fact]
        public void ShouldReleaseServiceProvider()
        {
            var customerLocation = new Geolocation() { Y = 50.1162, X = 122.9535 };
            var customer = new Customer() { CustomerId = Guid.NewGuid() };
            var assistant = _service.ReserveAssistant(customer, customerLocation);
            assistant?.AssistantId.Should().Be(_providers[1].ServiceProviderId); // Vancouver                
            _providers[1].CustomerId.Should().Be(customer.CustomerId);

            _service.ReleaseAssistant(customer, assistant!);
            _providers[1].CustomerId.Should().BeNull();
        }

        [Fact]
        public void ShouldThrowIfServiceProviderIsNotPresentDuringRelease()
        {
            var customerLocation = new Geolocation() { Y = 50.1162, X = 122.9535 };
            var customer = new Customer() { CustomerId = Guid.NewGuid() };

            var method = () => _service.ReleaseAssistant(customer, new Assistant() { AssistantId = Guid.NewGuid() });
            method.Should().Throw<ServiceProviderNotFoundException>();
        }

        [Fact]
        public void ShouldThrowIfCustomerIsNotAssignedToServiceProviderDuringRelease()
        {
            var customerLocation = new Geolocation() { Y = 50.1162, X = 122.9535 };
            var customer = new Customer() { CustomerId = Guid.NewGuid() };
            var assistant = _service.ReserveAssistant(customer, customerLocation);
            assistant?.AssistantId.Should().Be(_providers[1].ServiceProviderId); // Vancouver                
            _providers[1].CustomerId.Should().Be(customer.CustomerId);

            var method = () => _service.ReleaseAssistant(new Customer() { CustomerId = Guid.NewGuid() }, assistant!);
            method.Should().Throw<CustomerNotAssignedToServiceProviderException>();
        }
    }
}
