using FastEndpoints;

using FoodCalc.Features.Recipes.Commands.AddIngredientToRecipe;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>POST api/recipe/ingredient — Admin only.</summary>
public class AddIngredientToRecipeEndpoint(IMediator mediator)
	: Endpoint<RecipeIngredientDto, RecipeIngredientDto>
{
	public override void Configure()
	{
		Post("api/recipe/ingredient");
		Roles("Admin");
	}

	public override async Task HandleAsync(RecipeIngredientDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new AddIngredientToRecipeCommand(req), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
