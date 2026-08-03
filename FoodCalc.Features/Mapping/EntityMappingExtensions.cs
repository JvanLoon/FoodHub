using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Features.Mapping;

/// <summary>
/// Hand-written entity &lt;-&gt; DTO mapping, replacing AutoMapper. Kept intentionally
/// explicit: each conversion is a plain constructor/assignment, so there is no
/// runtime mapping engine (and no uncontrolled recursion — cf. CVE-2026-32933).
/// Behaviour mirrors the previous AutoMapper profile, including turning a null
/// source collection into an empty list rather than null.
/// </summary>
public static class EntityMappingExtensions
{
    // ---------- Ingredient ----------
    public static IngredientDto ToDto(this Ingredient e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        ShouldBeAddedToShoppingCart = e.ShouldBeAddedToShoppingCart,
        CreatedByUserId = e.CreatedByUserId,
        IsReviewed = e.IsReviewed
    };

    public static List<IngredientDto> ToDtoList(this IEnumerable<Ingredient> items) => items.Select(i => i.ToDto())
        .ToList();

    public static Ingredient ToEntity(this IngredientDto d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        ShouldBeAddedToShoppingCart = d.ShouldBeAddedToShoppingCart,
        CreatedByUserId = d.CreatedByUserId
        // IsReviewed is not mapped from the client: approval is granted by the review
        // endpoints alone, never by echoing a flag back in a request body.
    };

    // CreateIngredientDto leaves Id/CreatedDate/ModifiedDate at their entity defaults, and
    // IsReviewed at false — a new entry is always unapproved. The caller sets CreatedByUserId.
    public static Ingredient ToEntity(this CreateIngredientDto d) => new()
    {
        Name = d.Name,
        ShouldBeAddedToShoppingCart = d.ShouldBeAddedToShoppingCart
    };

    // ---------- RecipeItem ----------
    public static RecipeItemDto ToDto(this RecipeItem e) => new()
    {
        Id = e.Id,
        RecipeId = e.RecipeId,
        IngredientId = e.IngredientId,
        Ingredient = e.Ingredient.ToDto(),
        Amount = e.Amount,
        IngredientAmount = (IngredientAmountTypeDto) e.IngredientAmount,
        ShouldBeAddedToShoppingCart = e.ShouldBeAddedToShoppingCart
    };

    public static PendingRecipeItemDto ToPendingDto(this RecipeItem e) => new()
    {
        Id = e.Id,
        IngredientId = e.IngredientId,
        Ingredient = e.Ingredient.ToDto(),
        Amount = e.Amount,
        IngredientAmount = (IngredientAmountTypeDto) e.IngredientAmount,
        IsChanged = !e.IsReviewed
    };

    public static List<RecipeItemDto> ToDtoList(this IEnumerable<RecipeItem> items) => items.Select(ri => ri.ToDto())
        .ToList();

    public static RecipeItem ToEntity(this RecipeItemDto d) => new()
    {
        Id = d.Id,
        RecipeId = d.RecipeId,
        IngredientId = d.IngredientId,
        Amount = d.Amount,
        IngredientAmount = (IngredientAmountType) d.IngredientAmount,
        ShouldBeAddedToShoppingCart = d.ShouldBeAddedToShoppingCart
    };

    // Copies the editable fields onto an existing tracked entity, leaving its
    // identity (Id/RecipeId) untouched. Used when reconciling a recipe's items.
    //
    // Compares before assigning so that a line submitted unchanged stays Unmodified in the
    // change tracker. Two things depend on that: the line keeps its approved status instead of
    // being flagged as edited on the review screen, and UpdateRecipeCommandHandler can ask the
    // tracker whether anything actually changed rather than assuming it did.
    public static void ApplyTo(this RecipeItemDto d, RecipeItem e)
    {
        var amountType = (IngredientAmountType) d.IngredientAmount;

        var changed = e.Ingredient.Name != d.Ingredient.Name || e.IngredientId != d.IngredientId || e.Amount != d.Amount ||
                      e.IngredientAmount != amountType ||
                      e.ShouldBeAddedToShoppingCart != d.ShouldBeAddedToShoppingCart;

        if (!changed)
            return;

        e.Ingredient = d.Ingredient.ToEntity();
        e.IngredientId = d.IngredientId;
        e.Amount = d.Amount;
        e.IngredientAmount = amountType;
        e.ShouldBeAddedToShoppingCart = d.ShouldBeAddedToShoppingCart;
        e.IsReviewed = false;
    }

    // ---------- Recipe ----------
    public static RecipeDto ToDto(this Recipe e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        CreatedByUserId = e.CreatedByUserId,
        IsReviewed = e.IsReviewed,
        Ingredients = [..e.Ingredients.Select(ri => ri.ToDto())]
    };

    /// <summary>Projection for the review queue: every line, with the changed ones flagged.</summary>
    public static PendingRecipeDto ToPendingDto(this Recipe e, string createdByEmail) => new()
    {
        Id = e.Id,
        Name = e.Name,
        CreatedByUserId = e.CreatedByUserId,
        CreatedByEmail = createdByEmail,
        CreatedDate = e.CreatedDate,
        ModifiedDate = e.ModifiedDate,
        IsFirstSubmission = e.FirstApprovedDate is null,
        Ingredients = e.Ingredients.OrderBy(ri => ri.Ingredient.Name).Select(ri => ri.ToPendingDto()).ToList()
    };

    public static List<RecipeDto> ToDtoList(this IEnumerable<Recipe> items) => items.Select(r => r.ToDto())
        .ToList();

    // CreateRecipeDto leaves Id/CreatedDate/ModifiedDate/RecipeItem at defaults.
    public static Recipe ToEntity(this CreateRecipeDto d) => new()
    {
        Name = d.Name
    };

    // ---------- MealPlanEntry ----------
    public static MealPlanEntryDto ToDto(this MealPlanEntry e) => new()
    {
        Id = e.Id,
        Date = e.Date,
        RecipeId = e.RecipeId,
        RecipeName = e.Recipe?.Name ?? string.Empty
    };

    public static List<MealPlanEntryDto> ToDtoList(this IEnumerable<MealPlanEntry> items) =>
    [
        ..items.Select(m => m.ToDto())
    ];

    // ---------- User ----------
    // Roles are populated separately by the caller (as before).
    public static UserDto ToUserDto(this IdentityUser u) => new()
    {
        Id = u.Id,
        Name = u.UserName ?? string.Empty,
        Email = u.Email ?? string.Empty,
        // Enable/disable is gated on EmailConfirmed (see ToggleUserEndpoint / LoginEndpoint).
        Enabled = u.EmailConfirmed,
        EmailConfirmed = u.EmailConfirmed,
        Roles = []
    };
}