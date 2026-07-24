namespace FoodHub.Persistence.Entities;

public class Recipe : BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Id of the <see cref="Microsoft.AspNetCore.Identity.IdentityUser"/> who created this
	/// recipe (string key, same convention as <see cref="MealPlanEntry.UserId"/>). Required:
	/// creation is rejected without a logged-in user, and legacy rows were backfilled to the
	/// first admin by migration.
	/// </summary>
	public string CreatedByUserId { get; set; } = string.Empty;

	public List<RecipeItem> Ingredients { get; set; } = [];
}