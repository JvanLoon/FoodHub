using FastEndpoints;

using FluentValidation;

using FoodCalc.Api.Endpoints.Common;

using FoodHub.DTOs;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>Paging guard for GET api/recipe/getallrecipes (see <see cref="PagedSearchRequestValidator{T}"/>).</summary>
public class GetRecipesRequestValidator : PagedSearchRequestValidator<GetRecipesRequest>;

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
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}

public class UpdateRecipeValidator : Validator<UpdateRecipeDto>
{
	public UpdateRecipeValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.EntityIdRequired(Entity.Recipe));

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}

public class RecipeNameUpdateValidator : Validator<RecipeNameUpdateDto>
{
	public RecipeNameUpdateValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.EntityIdRequired(Entity.Recipe));

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}
