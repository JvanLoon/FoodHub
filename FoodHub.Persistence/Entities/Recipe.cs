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

    /// <summary>
    /// False until a moderator approves the recipe. Anyone other than the author sees only
    /// reviewed recipes, so a new — or newly edited — recipe is private to its author until
    /// it is approved. Deliberately defaults to false: every write path must opt in to
    /// visibility through review, never by forgetting to set it.
    /// </summary>
    public bool IsReviewed { get; set; }

    /// <summary>
    /// When this recipe was first approved, or null if it never has been. Distinct from
    /// <see cref="IsReviewed"/>, which goes back to false on every edit: this one is set once
    /// and kept, so the review queue can tell a brand-new submission from an edit to something
    /// that is already published — two cases a moderator judges very differently.
    /// </summary>
    public DateTime? FirstApprovedDate { get; set; }

    public List<RecipeItem>? Ingredients { get; set; } = [];
}