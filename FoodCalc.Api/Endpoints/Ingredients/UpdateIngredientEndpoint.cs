using FastEndpoints;

using FluentValidation;

using FoodCalc.Features.Ingredients.Commands.UpdateIngredient;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Ingredients;

public class UpdateIngredientValidator : Validator<UpdateIngredientDto>
{
	public UpdateIngredientValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("No ingredient id provided");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("No name provided");
	}
}

/// <summary>PUT api/ingredient — Admin only.</summary>
public class UpdateIngredientEndpoint(IMediator mediator)
	: Endpoint<UpdateIngredientDto, IngredientDto>
{
	public override void Configure()
	{
		Put("api/ingredient");
		Roles("Admin");
	}

	public override async Task HandleAsync(UpdateIngredientDto req, CancellationToken ct)
	{
		var result = await mediator.Send(new UpdateIngredientCommand(req), ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
