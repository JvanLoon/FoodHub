using FastEndpoints;

using FoodCalc.Features.Recipes.Commands.DeleteRecipe;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>DELETE api/recipe/deleterecipe/{id} — Admin only.</summary>
public class DeleteRecipeEndpoint(IMediator mediator)
	: Endpoint<RecipeByIdRequest, bool>
{
	public override void Configure()
	{
		Delete(ApiRoutes.Recipe.DeleteRecipeTemplate);
		Roles("Admin");
	}

	public override async Task HandleAsync(RecipeByIdRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new DeleteRecipeCommand(req.Id), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
