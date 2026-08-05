using ErrorOr;
using FoodHub.DTOs;
using MediatR;

namespace FoodCalc.Features.Ingredients.Commands.AddIngredient;

/// <param name="CreatedByUserId">
/// Author of the new catalog entry. Required — an entry with no author could never be seen by
/// anyone before approval, including whoever added it.
/// </param>
public record AddIngredientCommand(string IngredientName, string? CreatedByUserId = null)
    : IRequest<ErrorOr<IngredientDto>>;