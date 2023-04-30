namespace RoadSideAssistance.Application.Exceptions
{
    public class CustomerAlreadyAssignedToServiceProviderException : Exception
    {
        public CustomerAlreadyAssignedToServiceProviderException(Guid customerId, Guid serviceProviderId) :
            base($"Customer {customerId} already assigned to Service Provider {serviceProviderId}")
        {
            CustomerId = customerId;
            ServiceProviderId = serviceProviderId;
        }

        public Guid CustomerId { get; private set; }

        public Guid ServiceProviderId { get; private set; }
    }
}
