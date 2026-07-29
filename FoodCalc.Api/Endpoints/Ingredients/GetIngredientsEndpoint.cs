using FastEndpoints;
using FoodCalc.Feature.Ingredients.Queries.GetAllIngredients;
using MediatR;

namespace FoodCalc.Api.Endpoints.Ingredients;

/// <summary>Query parameters for GET api/ingredient.</summary>
public class GetIngredientsRequest : PagedSearchRequest;

/// <summary>GET api/ingredient — any authenticated user.</summary>
public class GetIngredientsEndpoint(IMediator mediator) : Endpoint<GetIngredientsRequest, PagedResultDto<IngredientDto>>
{
    public override void Configure()
    {
        Get(ApiRoutes.Ingredient.GetAll);
        // [Authorize] on the original action: authenticated user, any role.
    }

    public override async Task HandleAsync(GetIngredientsRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllIngredientsQuery
        {
            Page = req.Page,
            PageSize = req.PageSize,
            Search = req.Search,
            RequestingUserId = User.GetUserId()
        }, ct);

        await result.Match(value => Send.OkAsync(value, ct), errors => this.SendErrorsAsync(errors, ct: ct));
    }
}