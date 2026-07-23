namespace FoodHub.Persistence.Entities;

/// <summary>
/// A single "eat this recipe on this day" entry on a user's meal calendar.
/// Scoped to an <see cref="Microsoft.AspNetCore.Identity.IdentityUser"/> by
/// <see cref="UserId"/>. A day may hold multiple entries (up to a business cap of 20).
/// </summary>
public class MealPlanEntry : BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();

	/// <summary>Owning IdentityUser id (string key).</summary>
	public string UserId { get; set; } = string.Empty;

	/// <summary>The calendar day this recipe is planned for (date only, no time).</summary>
	public DateOnly Date { get; set; }

	public Guid RecipeId { get; set; }

	public Recipe? Recipe { get; set; }
}
