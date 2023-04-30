using Ardalis.GuardClauses;
using RoadSideAssistance.Domain.Entities;

namespace RoadSideAssistance.Application.GuardClauses
{
    public static class CustomerGuard
    {
        public static void Invalid(this IGuardClause guardClause, Customer input, string parameterName)
        {
            if (input.CustomerId.Equals(Guid.Empty))
                throw new ArgumentException($"Customer Guid {input.CustomerId} is invalid", parameterName);
        }
    }
}
