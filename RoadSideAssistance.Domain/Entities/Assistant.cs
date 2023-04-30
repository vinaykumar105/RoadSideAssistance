namespace RoadSideAssistance.Domain.Entities
{
    public record Assistant: IComparable<Assistant>
    {
        public Guid AssistantId { get; init; }

        public double DistanceFromCustomer { get; init; }

        public int CompareTo(Assistant? other)
        {
            return DistanceFromCustomer.CompareTo(other?.DistanceFromCustomer);
        }
    }
}
