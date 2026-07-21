using FastEndpoints;

using FoodCalc.Features.Recipes.Commands.UpdateRecipe;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>PUT api/recipe — Admin only. Body validated by UpdateRecipeValidator.</summary>
public class UpdateRecipeEndpoint(IMediator mediator)
	: Endpoint<UpdateRecipeDto, RecipeDto>
{
	public override void Configure()
	{
		Put(ApiRoutes.Recipe.Update);
		Roles("Admin");
	}

	public override async Task HandleAsync(UpdateRecipeDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new UpdateRecipeCommand(req), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
