using FastEndpoints;

using FluentValidation;

using FoodHub.DTOs;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>
/// Replaces the old inline ModelState check (`if (string.IsNullOrEmpty(recipe.Name))`)
/// in the AddRecipe controller action.
/// </summary>
public class CreateRecipeValidator : Validator<CreateRecipeDto>
{
	public CreateRecipeValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("No name provided");
	}
}

public class UpdateRecipeValidator : Validator<UpdateRecipeDto>
{
	public UpdateRecipeValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("No recipe id provided");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("No name provided");
	}
}

public class RecipeNameUpdateValidator : Validator<RecipeNameUpdateDto>
{
	public RecipeNameUpdateValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("No recipe id provided");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("No name provided");
	}
}
