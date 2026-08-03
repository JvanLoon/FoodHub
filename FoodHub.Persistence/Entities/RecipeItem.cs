namespace FoodHub.Persistence.Entities;

/// <summary>
/// An ingredient line that belongs to a single <see cref="Recipe"/>. This is not a
/// link table: the name (and the shopping-cart flag) are snapshotted onto the line,
/// so a recipe exposes its ingredients directly as recipe.Ingredients[i].Name and
/// survives the removal of a catalog entry. The <see cref="Ingredient"/> entity is
/// the separate catalog used for autocomplete/management, and
/// <see cref="IngredientId"/> points back at it so lines can be compared by identity.
/// </summary>
public class RecipeItem : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public decimal Amount { get; set; }
    public IngredientAmountType IngredientAmount { get; set; }
    public bool ShouldBeAddedToShoppingCart { get; set; } = true;

    /// <summary>
    /// False while this line is waiting on review. It does not hide the line — a line is only
    /// ever seen through its <see cref="Recipe"/>, which carries its own gate. It exists so the
    /// review screen can list every line of a recipe and mark the ones that actually changed,
    /// rather than showing a whole recipe with no indication of what to look at.
    /// </summary>
    public bool IsReviewed { get; set; }
}