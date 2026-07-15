using FastEndpoints;

using FluentValidation;

using FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Api.Endpoints.Ingredients;

/// <summary>Query parameters for GET api/ingredient.</summary>
public class GetIngredientsRequest
{
	[BindFrom("page")]
	public int Page { get; set; } = 1;

	[BindFrom("pageSize")]
	public int PageSize { get; set; } = 25;

	[BindFrom("search")]
	public string? Search { get; set; }
}

/// <summary>Paging guard (PageSize lower-bounded only; see GetRecipesRequestValidator).</summary>
public class GetIngredientsRequestValidator : Validator<GetIngredientsRequest>
{
	public GetIngredientsRequestValidator()
	{
		RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
		RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
	}
}

/// <summary>GET api/ingredient — any authenticated user.</summary>
public class GetIngredientsEndpoint(IMediator mediator)
	: Endpoint<GetIngredientsRequest, PagedResultDto<IngredientDto>>
{
	public override void Configure()
	{
		Get("api/ingredient");
		// [Authorize] on the original action: authenticated user, any role.
	}

	public override async Task HandleAsync(GetIngredientsRequest req, CancellationToken ct)
	{
		var result = await mediator.Send(
			new GetAllIngredientsQuery { Page = req.Page, PageSize = req.PageSize, Search = req.Search }, ct);

		await result.Match(
			value => Send.OkAsync(value, ct),
			errors => Send.ResultAsync(TypedResults.Problem(errors.First().Description)));
	}
}
