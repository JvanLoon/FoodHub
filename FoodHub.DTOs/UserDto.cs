namespace FoodHub.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Enabled { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// True while the account is signed in and still sending heartbeats. The server decides this
    /// — the client must not re-derive it from <see cref="LastSeenUtc"/>, or the two would
    /// disagree for anyone who signed out within the presence window.
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// When the account was last active, in UTC. Null for an account that has never signed in.
    /// Only meaningful to show while <see cref="IsOnline"/> is false.
    /// </summary>
    public DateTime? LastSeenUtc { get; set; }
}