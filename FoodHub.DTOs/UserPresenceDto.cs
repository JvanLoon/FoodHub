namespace FoodHub.DTOs;

/// <summary>
/// Just the presence half of <see cref="UserDto"/>, for the admin list's periodic refresh. Kept
/// separate so polling costs one small query instead of re-reading every account and its roles.
/// </summary>
public class UserPresenceDto
{
    public string UserId { get; set; } = null!;

    public bool IsOnline { get; set; }

    public DateTime? LastSeenUtc { get; set; }
}
