using ErrorOr;
using FoodHub.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Presence.Queries.GetPresence;

public class GetPresenceQueryHandler(
    FoodHubDbContext context,
    ILogger<GetPresenceQueryHandler> logger) : IRequestHandler<GetPresenceQuery, ErrorOr<List<UserPresenceDto>>>
{
    /// <summary>
    /// Ceiling on one request, matched to the user list's largest page size. Without it the
    /// polling endpoint would happily take an unbounded id list from any admin session.
    /// </summary>
    private const int MaxIds = 200;

    public async Task<ErrorOr<List<UserPresenceDto>>> Handle(GetPresenceQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIds = request.UserIds.Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .Take(MaxIds)
                .ToList();

            if (userIds.Count == 0)
                return new List<UserPresenceDto>();

            var rows = await context.UserPresences.Where(p => userIds.Contains(p.UserId))
                .ToDictionaryAsync(p => p.UserId, cancellationToken);

            // Judged against a single instant, so two accounts on the same page cannot land on
            // opposite sides of the timeout.
            var now = DateTime.UtcNow;

            return userIds.Select(id =>
                {
                    var seen = rows.GetValueOrDefault(id);
                    return new UserPresenceDto
                    {
                        UserId = id,
                        IsOnline = PresenceWindow.IsOnline(seen, now),
                        LastSeenUtc = seen?.LastSeenUtc
                    };
                })
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read presence for {Count} users", request.UserIds.Count);
            return Error.Failure(description: ErrorMessages.Common.GetAllFailed(ErrorMessages.Entities.Users));
        }
    }
}
