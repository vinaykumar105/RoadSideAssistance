namespace RoadSideAssistance.Domain.Entities
{
    public class RoadSideServiceProvider
    {
        public Guid ServiceProviderId { get; init; }

        public Guid? CustomerId { get; set; }

        public Geolocation Location { get; set; }

        /// <summary>
        /// Optimistic concurrency <TODO/>
        /// </summary>
        public byte[] RowVersion { get; set; } = new byte[0];
    }
}
