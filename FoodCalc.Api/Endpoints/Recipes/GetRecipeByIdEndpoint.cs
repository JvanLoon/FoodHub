using FastEndpoints;

using FoodCalc.Features.Recipes.Queries.GetById;

using MediatR;

namespace FoodCalc.Api.Endpoints.Recipes;

/// <summary>GET api/recipe/{id} — any authenticated user (read-only recipe view).</summary>
public class GetRecipeByIdEndpoint(IMediator mediator)
	: Endpoint<RecipeByIdRequest, RecipeDto?>
{
	public override void Configure()
	{
		Get(ApiRoutes.Recipe.GetByIdTemplate);
		Policies("Admin,Moderator,User");
	}

	public override async Task HandleAsync(RecipeByIdRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(new GetRecipeByIdQuery(req.Id), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
