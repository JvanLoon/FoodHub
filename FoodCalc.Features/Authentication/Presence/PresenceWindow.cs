using FoodHub.Persistence.Entities;

namespace FoodCalc.Features.Authentication.Presence;

/// <summary>
/// The one place that decides what "online" means, so the API, the heartbeat interval and the
/// admin list cannot drift apart.
/// </summary>
public static class PresenceWindow
{
    /// <summary>
    /// How long a heartbeat keeps an account green. Must be comfortably more than the client's
    /// heartbeat interval — a dropped request or a slow reconnect should not blink the dot off.
    /// </summary>
    public static readonly TimeSpan Timeout = TimeSpan.FromMinutes(3);

    /// <summary>
    /// How long the API waits before writing the same user's presence again. Deliberately shorter
    /// than the client heartbeat, so every heartbeat lands and the timestamp never coasts.
    /// </summary>
    public static readonly TimeSpan WriteInterval = TimeSpan.FromSeconds(45);

    /// <summary>
    /// True if the account should show as online. Both halves matter: <c>IsOnline</c> alone would
    /// stay true forever for a browser that was closed without signing out, and a recent
    /// timestamp alone would keep someone green for three minutes after they hit Uitloggen.
    /// </summary>
    public static bool IsOnline(UserPresence? presence, DateTime utcNow) =>
        presence is { IsOnline: true } && presence.LastSeenUtc >= utcNow - Timeout;
}
