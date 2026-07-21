using FastEndpoints;

using FoodCalc.Features.Recipes.Commands.AddRecipe;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>POST api/recipe — Admin only. Body validated by CreateRecipeValidator.</summary>
public class CreateRecipeEndpoint(IMediator mediator)
	: Endpoint<CreateRecipeDto, RecipeDto>
{
	public override void Configure()
	{
		Post(ApiRoutes.Recipe.Create);
		Roles("Admin");
	}

	public override async Task HandleAsync(CreateRecipeDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new AddRecipeCommand(req), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
