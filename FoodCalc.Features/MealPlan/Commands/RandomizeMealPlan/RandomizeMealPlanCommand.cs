using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.MealPlan.Commands.RandomizeMealPlan;

/// <summary>
/// Fills the given days with random recipes picked from the existing library.
/// <see cref="Ingredients"/> (optional) restricts the pool to recipes that use at
/// least one of them (falling back to the full library if none match).
/// <see cref="Overwrite"/> clears each day first; otherwise entries are appended,
/// respecting the per-day cap.
/// </summary>
public record RandomizeMealPlanCommand(
	string UserId,
	IReadOnlyList<DateOnly> Dates,
	IReadOnlyList<string> Ingredients,
	int RecipesPerDay,
	bool Overwrite) : IRequest<ErrorOr<List<MealPlanEntryDto>>>;
