using FastEndpoints;

using FoodCalc.Features.Recipes.Commands.UpdateRecipeName;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>PUT api/recipe/name — Admin only. Body validated by RecipeNameUpdateValidator.</summary>
public class UpdateRecipeNameEndpoint(IMediator mediator)
	: Endpoint<RecipeNameUpdateDto, RecipeDto>
{
	public override void Configure()
	{
		Put("api/recipe/name");
		Roles("Admin");
	}

	public override async Task HandleAsync(RecipeNameUpdateDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new UpdateRecipeNameCommand(req.Id, req.Name), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
