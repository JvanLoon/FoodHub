using FastEndpoints;

using FoodCalc.Features.Ingredients.Commands.DeleteIngredientFromRecipe;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>DELETE api/recipe/deleteingredient/{id} — Admin only.</summary>
public class DeleteIngredientEndpoint(IMediator mediator)
	: Endpoint<RecipeIngredientByIdRequest, bool>
{
	public override void Configure()
	{
		Delete(ApiRoutes.Recipe.DeleteIngredientTemplate);
		Roles("Admin");
	}

	public override async Task HandleAsync(RecipeIngredientByIdRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new DeleteIngredientFromRecipeCommand(req.Id), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
