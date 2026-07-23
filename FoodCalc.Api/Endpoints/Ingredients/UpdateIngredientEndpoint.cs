using FastEndpoints;

using FoodCalc.Features.Ingredients.Commands.UpdateIngredient;

using MediatR;

namespace FoodCalc.Api.Endpoints.Ingredients;

/// <summary>PUT api/ingredient — Admin only.</summary>
public class UpdateIngredientEndpoint(IMediator mediator) : Endpoint<UpdateIngredientDto, IngredientDto>
{
	public override void Configure()
	{
		Put(ApiRoutes.Ingredient.Update);
		Roles("Admin");
	}

	public override async Task HandleAsync(UpdateIngredientDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new UpdateIngredientCommand(req), ct);

		await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
	}
}
