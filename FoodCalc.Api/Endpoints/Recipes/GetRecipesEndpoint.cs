using FastEndpoints;

using FoodCalc.Features.Recipes.Queries.GetAllRecipes;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>GET api/recipe/getallrecipes — any authenticated user.</summary>
public class GetRecipesEndpoint(IMediator mediator)
	: Endpoint<GetRecipesRequest, PagedResultDto<RecipeDto>>
{
	public override void Configure()
	{
		Get("api/recipe/getallrecipes");
		// [Authorize] on the original action: authenticated user, any role.
	}

	public override async Task HandleAsync(GetRecipesRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(
			new GetAllRecipesQuery(req.WithIngredient, req.Page, req.PageSize, req.Search), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
