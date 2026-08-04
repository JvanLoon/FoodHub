namespace FoodHub.Persistence.Entities;

/// <summary>
/// Tracks when an account was last active, so the admin user list can show who is online.
///
/// A side table rather than a column on the user: Identity is used raw as
/// <see cref="Microsoft.AspNetCore.Identity.IdentityUser"/> throughout the solution, and
/// subclassing it to carry two fields would mean re-typing every UserManager, SignInManager and
/// the DbContext itself. Keyed by <see cref="UserId"/>, so there is exactly one row per account
/// and the write is an upsert.
/// </summary>
public class UserPresence
{
    /// <summary>Owning IdentityUser id (string key). Primary key — one row per account.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// When the account last made an authenticated call, always UTC. Kept even after signing out,
    /// which is the whole point: it is what "laatst actief 5 min geleden" is measured from.
    /// </summary>
    public DateTime LastSeenUtc { get; set; }

    /// <summary>
    /// False once the user signs out explicitly. On its own it is not enough — a closed tab or a
    /// killed browser never sends a sign-out and would leave this true forever — so a reader must
    /// require a recent <see cref="LastSeenUtc"/> as well. See PresenceWindow.IsOnline.
    /// </summary>
    public bool IsOnline { get; set; }
}
