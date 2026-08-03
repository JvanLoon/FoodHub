namespace FoodHub.DTOs;

/// <summary>
/// Everything currently awaiting moderator approval. Only recipes: the recipe is the review
/// gate, and its ingredients are approved along with it.
/// </summary>
public class ReviewQueueDto
{
    public List<PendingRecipeDto> Recipes { get; set; } = [];

    public int TotalCount => Recipes.Count;
}

/// <summary>A recipe waiting on approval, with enough context to judge it without opening it.</summary>
public class PendingRecipeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;

    /// <summary>Author's email, resolved from Identity. Falls back to the raw id if the account is gone.</summary>
    public string CreatedByEmail { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// True when the recipe has never been approved, i.e. this is a first submission rather
    /// than an edit to something already published.
    /// </summary>
    public bool IsFirstSubmission { get; set; }

    /// <summary>
    /// Every line of the recipe, not just the changed ones — a moderator judging "chicken:
    /// 4000 g" needs to see the rest of the dish. <see cref="PendingRecipeItemDto.IsChanged"/>
    /// marks which ones still need approving or rejecting; the recipe cannot be approved while
    /// any line is still changed.
    /// </summary>
    public List<PendingRecipeItemDto> Ingredients { get; set; } = [];
}

public class PendingRecipeItemDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public IngredientDto Ingredient { get; set; } = null!;
    public decimal Amount { get; set; }
    public IngredientAmountTypeDto IngredientAmount { get; set; }

    /// <summary>True if this line was added or edited since the recipe was last approved.</summary>
    public bool IsChanged { get; set; }
}

/// <summary>
/// Body of any approve or reject call — a recipe or a single recipe line. Approve publishes the
/// target; reject deletes it. Nothing else is carried: rejection records no reason or who.
/// </summary>
public class ReviewTargetDto
{
    public Guid Id { get; set; }
}
