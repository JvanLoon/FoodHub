using FastEndpoints;

using FoodCalc.Features.Ingredients.Commands.AddIngredient;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Ingredients;

/// <summary>POST api/ingredient — Admin only.</summary>
public class CreateIngredientEndpoint(IMediator mediator)
	: Endpoint<CreateIngredientDto, IngredientDto>
{
	public override void Configure()
	{
		Post(ApiRoutes.Ingredient.Create);
		Roles("Admin");
	}

	public override async Task HandleAsync(CreateIngredientDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new AddIngredientCommand(req), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
