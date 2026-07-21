using FastEndpoints;

using FoodCalc.Features.Recipes.Queries.GetById;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>GET api/recipe/{id} — Admin only.</summary>
public class GetRecipeByIdEndpoint(IMediator mediator)
	: Endpoint<RecipeByIdRequest, RecipeDto?>
{
	public override void Configure()
	{
		Get(ApiRoutes.Recipe.GetByIdTemplate);
		Roles("Admin");
	}

	public override async Task HandleAsync(RecipeByIdRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new GetRecipeByIdQuery(req.Id), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
