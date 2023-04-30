namespace RoadSideAssistance.Application.Exceptions
{
    public class CustomerNotAssignedToServiceProviderException : Exception
    {
        public CustomerNotAssignedToServiceProviderException(Guid customerId, Guid serviceProviderId) :
            base($"Customer {customerId} not assigned to Service Provider {serviceProviderId}")
        {
            CustomerId = customerId;
            ServiceProviderId = serviceProviderId;
        }

        public Guid CustomerId { get; private set; }

        public Guid ServiceProviderId { get; private set; }
    }
}
