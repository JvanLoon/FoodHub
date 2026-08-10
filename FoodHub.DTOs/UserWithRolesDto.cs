namespace FoodHub.DTOs;

public class UserWithRolesDto
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = default!;
    public string SecurityStamp { get; set; } = default!;
    public string ConcurrencyStamp { get; set; } = default!;
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public List<string> Roles { get; set; } = [];
}