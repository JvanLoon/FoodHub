namespace FoodHub.Persistence.Entities;

public class Ingredient : BaseEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public bool ShouldBeAddedToShoppingCart { get; set; } = true;

    /// <summary>
    /// Id of the <see cref="Microsoft.AspNetCore.Identity.IdentityUser"/> who added this
    /// catalog entry, same string-key convention as <see cref="Recipe.CreatedByUserId"/>.
    /// Empty on the rows that predate review — those were all approved by migration, so
    /// nothing depends on their author.
    /// </summary>
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// False until a moderator approves the entry. Mirrors <see cref="Recipe.IsReviewed"/>:
    /// a newly added ingredient is visible only to whoever added it until it is approved.
    /// </summary>
    public bool IsReviewed { get; set; }

    /// <inheritdoc cref="Recipe.FirstApprovedDate"/>
    public DateTime? FirstApprovedDate { get; set; }
}