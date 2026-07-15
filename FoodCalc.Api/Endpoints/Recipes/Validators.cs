using FastEndpoints;

using FluentValidation;

using FoodHub.DTOs;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>
/// Paging guard. PageSize is only lower-bounded: the Blazor "get all" helpers
/// call this with pageSize = int.MaxValue to fetch every row, so an upper cap
/// would break them.
/// </summary>
public class GetRecipesRequestValidator : Validator<GetRecipesRequest>
{
	public GetRecipesRequestValidator()
	{
		RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
		RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
	}
}

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
			.WithMessage(ValidationMessages.NameRequired);
	}
}

public class UpdateRecipeValidator : Validator<UpdateRecipeDto>
{
	public UpdateRecipeValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ValidationMessages.RecipeIdRequired);

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.NameRequired);
	}
}

public class RecipeNameUpdateValidator : Validator<RecipeNameUpdateDto>
{
	public RecipeNameUpdateValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ValidationMessages.RecipeIdRequired);

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.NameRequired);
	}
}
