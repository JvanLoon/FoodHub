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
		ShouldBeAddedToShoppingCart = e.ShouldBeAddedToShoppingCart
	};

	public static List<IngredientDto> ToDtoList(this IEnumerable<Ingredient> items)
		=> items.Select(i => i.ToDto()).ToList();

	public static Ingredient ToEntity(this IngredientDto d) => new()
	{
		Id = d.Id,
		Name = d.Name,
		ShouldBeAddedToShoppingCart = d.ShouldBeAddedToShoppingCart
	};

	// CreateIngredientDto leaves Id/CreatedDate/ModifiedDate at their entity defaults.
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
		Name = e.Name,
		Amount = e.Amount,
		IngredientAmount = (IngredientAmountTypeDto)e.IngredientAmount,
		ShouldBeAddedToShoppingCart = e.ShouldBeAddedToShoppingCart
	};

	public static List<RecipeItemDto> ToDtoList(this IEnumerable<RecipeItem> items)
		=> items.Select(ri => ri.ToDto()).ToList();

	public static RecipeItem ToEntity(this RecipeItemDto d) => new()
	{
		Id = d.Id,
		RecipeId = d.RecipeId,
		Name = d.Name,
		Amount = d.Amount,
		IngredientAmount = (IngredientAmountType)d.IngredientAmount,
		ShouldBeAddedToShoppingCart = d.ShouldBeAddedToShoppingCart
	};

	// ---------- Recipe ----------
	public static RecipeDto ToDto(this Recipe e) => new()
	{
		Id = e.Id,
		Name = e.Name ?? string.Empty,
		// Null collection -> empty list (GetAllRecipes nulls this out when
		// WithIngredient is false), matching the old AutoMapper behaviour.
		Ingredients = e.Ingredients?.Select(ri => ri.ToDto()).ToList() ?? new()
	};

	public static List<RecipeDto> ToDtoList(this IEnumerable<Recipe> items)
		=> items.Select(r => r.ToDto()).ToList();

	// CreateRecipeDto leaves Id/CreatedDate/ModifiedDate/RecipeItem at defaults.
	public static Recipe ToEntity(this CreateRecipeDto d) => new()
	{
		Name = d.Name
	};

	// ---------- User ----------
	// Roles are populated separately by the caller (as before).
	public static UserDto ToUserDto(this IdentityUser u) => new()
	{
		Id = u.Id,
		Name = u.UserName ?? string.Empty,
		Email = u.Email ?? string.Empty,
		Enabled = u.LockoutEnabled,
		EmailConfirmed = u.EmailConfirmed,
		Roles = []
	};
}
