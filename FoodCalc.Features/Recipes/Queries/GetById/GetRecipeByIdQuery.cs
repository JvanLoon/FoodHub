using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Recipes.Queries.GetById;

/// <param name="RequestingUserId">
/// Caller's IdentityUser id. Fetching an unapproved recipe you do not own reports "not found"
/// rather than "forbidden" — the existence of someone else's pending recipe is itself private.
/// </param>
public record GetRecipeByIdQuery(Guid Id, string? RequestingUserId = null) : IRequest<ErrorOr<RecipeDto?>>;