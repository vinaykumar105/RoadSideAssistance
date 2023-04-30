namespace RoadSideAssistance.Application.Exceptions
{
    public class ServiceProviderNotFoundException: Exception
    {
        public ServiceProviderNotFoundException(Guid serviceProviderid):
            base($"Service Provider {serviceProviderid} not found")
        {
            ServiceProviderId = serviceProviderid;
        }

        public Guid ServiceProviderId { get; private set; }
    }
}
