namespace FoodHub.DTOs;

/// <summary>Everything currently awaiting moderator approval, in one round trip.</summary>
public class ReviewQueueDto
{
    public List<PendingRecipeDto> Recipes { get; set; } = [];
    public List<PendingIngredientDto> Ingredients { get; set; } = [];

    public int TotalCount => Recipes.Count + Ingredients.Count;
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
    /// marks which ones actually need attention.
    /// </summary>
    public List<PendingRecipeItemDto> Ingredients { get; set; } = [];

    /// <summary>The most recent rejection of this recipe, if it has been rejected and kept before.</summary>
    public ReviewRejectionDto? LastRejection { get; set; }
}

public class PendingRecipeItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public IngredientAmountTypeDto IngredientAmount { get; set; }

    /// <summary>True if this line was added or edited since the recipe was last approved.</summary>
    public bool IsChanged { get; set; }
}

/// <summary>A catalog ingredient waiting on approval.</summary>
public class PendingIngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool ShouldBeAddedToShoppingCart { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string CreatedByEmail { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public ReviewRejectionDto? LastRejection { get; set; }
}

/// <summary>A past rejection, shown so a moderator can tell a resubmission from a fresh one.</summary>
public class ReviewRejectionDto
{
    public string Reason { get; set; } = string.Empty;
    public string RejectedByEmail { get; set; } = string.Empty;
    public DateTime RejectedDate { get; set; }
    public bool TargetDeleted { get; set; }
}

/// <summary>Body of an approve call.</summary>
public class ApproveReviewDto
{
    public Guid Id { get; set; }
}

/// <summary>Body of a reject call. <see cref="Reason"/> is required.</summary>
public class RejectReviewDto
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = string.Empty;

    /// <summary>True to delete the rejected item outright; false to leave it with its author.</summary>
    public bool Delete { get; set; }
}
