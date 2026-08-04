using System.Collections.Concurrent;
using FoodCalc.Features.Authentication.Presence;

namespace FoodCalc.Api.Common;

/// <summary>
/// Rate-limits presence writes. Every authenticated request would otherwise mean a database
/// round-trip purely to move a timestamp a few milliseconds, so a user is written at most once
/// per <see cref="PresenceWindow.WriteInterval"/>.
///
/// Registered as a singleton and held in memory on purpose: losing it on restart costs one extra
/// write per active user, and the durable answer already lives in the UserPresence table.
/// </summary>
public class PresenceThrottle
{
    private readonly ConcurrentDictionary<string, DateTime> _lastWrittenUtc = new();

    /// <summary>
    /// True if this user is due for a write, and claims the slot so concurrent requests from the
    /// same account do not all decide yes.
    /// </summary>
    public bool ShouldWrite(string userId)
    {
        var now = DateTime.UtcNow;
        var claimed = false;

        _lastWrittenUtc.AddOrUpdate(userId, _ =>
        {
            claimed = true;
            return now;
        }, (_, previous) =>
        {
            if (now - previous < PresenceWindow.WriteInterval)
                return previous;

            claimed = true;
            return now;
        });

        return claimed;
    }

    /// <summary>
    /// Drops the user's slot so the next write goes through unthrottled. Used on sign-out, where
    /// the whole point is that the change lands immediately.
    /// </summary>
    public void Clear(string userId) => _lastWrittenUtc.TryRemove(userId, out _);
}
