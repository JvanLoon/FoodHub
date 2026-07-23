using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.MealPlan.Commands.AddMealPlanEntry;

public record AddMealPlanEntryCommand(string UserId, DateOnly Date, Guid RecipeId)
	: IRequest<ErrorOr<MealPlanEntryDto>>;
