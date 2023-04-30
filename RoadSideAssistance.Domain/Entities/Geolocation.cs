namespace RoadSideAssistance.Domain.Entities
{
    public record struct Geolocation
    {
        public double X { get; init; }
        public double Y { get; init; }
    }
}
