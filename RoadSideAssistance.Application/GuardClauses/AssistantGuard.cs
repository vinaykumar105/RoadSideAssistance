using Ardalis.GuardClauses;
using RoadSideAssistance.Domain.Entities;

namespace RoadSideAssistance.Application.GuardClauses
{
    public static class AssistantGuard
    {
        public static void Invalid(this IGuardClause guardClause, Assistant input, string parameterName)
        {
            if (input.AssistantId.Equals(Guid.Empty))
                throw new ArgumentException($"Customer Guid {input.AssistantId} is invalid", parameterName);
        }
    }
}
