using ErrorOr;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Presence.Commands.TouchPresence;

public class TouchPresenceCommandHandler(
    FoodHubDbContext context,
    ILogger<TouchPresenceCommandHandler> logger) : IRequestHandler<TouchPresenceCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(TouchPresenceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;

            // Update-then-insert rather than load-modify-save: this runs on every heartbeat from
            // every signed-in tab, so the common path is a single UPDATE with nothing tracked.
            var updated = await context.UserPresences.Where(p => p.UserId == request.UserId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.LastSeenUtc, now)
                    .SetProperty(p => p.IsOnline, request.IsOnline), cancellationToken);

            if (updated > 0)
                return true;

            context.UserPresences.Add(new UserPresence
            {
                UserId = request.UserId,
                LastSeenUtc = now,
                IsOnline = request.IsOnline
            });

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            // Two first-ever requests for the same account can both find no row and both insert;
            // one loses on the primary key. The winner wrote the same timestamp we would have, so
            // there is nothing to repair and nothing worth logging.
            context.ChangeTracker.Clear();
            return true;
        }
        catch (Exception ex)
        {
            // Presence is decoration. It must never take down the request that triggered it, so
            // the failure is logged and swallowed rather than surfaced to the caller.
            logger.LogWarning(ex, "Failed to record presence for user {UserId}", request.UserId);
            return true;
        }
    }
}
