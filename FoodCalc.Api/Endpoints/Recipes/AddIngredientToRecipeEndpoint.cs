using FastEndpoints;
using FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;
using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>POST api/recipe/ingredient — Admin only.</summary>
public class AddIngredientToRecipeEndpoint(IMediator mediator) : Endpoint<RecipeItemDto, RecipeItemDto>
{
	public override void Configure()
	{
		Post(ApiRoutes.Recipe.AddIngredient);
		Roles("Admin");
	}

	public override async Task HandleAsync(RecipeItemDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new AddIngredientToRecipeCommand(req), ct);

		await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
	}
}