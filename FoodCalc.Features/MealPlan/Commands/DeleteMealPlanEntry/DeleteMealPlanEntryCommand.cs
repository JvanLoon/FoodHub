using ErrorOr;

using MediatR;

namespace FoodCalc.Features.MealPlan.Commands.DeleteMealPlanEntry;

/// <summary>Deletes one of the calling user's own entries. UserId scopes the delete.</summary>
public record DeleteMealPlanEntryCommand(string UserId, Guid Id) : IRequest<ErrorOr<bool>>;
