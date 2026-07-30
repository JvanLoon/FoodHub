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
/// <see cref="UniqueOnly"/> spends each recipe once across the whole request, so a period
/// gets a different recipe every day.
/// </summary>
public record RandomizeMealPlanCommand(
    string UserId,
    IReadOnlyList<DateOnly> Dates,
    IReadOnlyList<string> Ingredients,
    int RecipesPerDay,
    bool Overwrite,
    bool UniqueOnly) : IRequest<ErrorOr<List<MealPlanEntryDto>>>;