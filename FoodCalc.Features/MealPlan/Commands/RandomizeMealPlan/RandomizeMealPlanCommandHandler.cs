using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodCalc.Features.Review;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.MealPlan.Commands.RandomizeMealPlan;

public class RandomizeMealPlanCommandHandler(FoodHubDbContext context, ILogger<RandomizeMealPlanCommandHandler> logger)
    : IRequestHandler<RandomizeMealPlanCommand, ErrorOr<List<MealPlanEntryDto>>>
{
    public async Task<ErrorOr<List<MealPlanEntryDto>>> Handle(RandomizeMealPlanCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dates = request.Dates.Distinct()
                .ToList();
            if (dates.Count == 0)
                return Error.Validation(description: ErrorMessages.MealPlan.NoDaysSelected);

            var perDay = Math.Clamp(request.RecipesPerDay, 1, MealPlanConstants.MaxRecipesPerDay);

            // Same visibility rule as browsing: randomize may pull in approved recipes plus
            // the caller's own pending ones, never another user's unapproved work.
            var allRecipes = await context.Recipes.VisibleTo(request.UserId)
                .ToListAsync(cancellationToken);
            if (allRecipes.Count == 0)
                return Error.Validation(description: ErrorMessages.MealPlan.NoRecipesToPickFrom);

            // Optional ingredient bias: keep recipes using any requested ingredient.
            var wanted = request.Ingredients.Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => i.Trim()
                    .ToLowerInvariant())
                .Distinct()
                .ToList();

            var pool = allRecipes;
            if (wanted.Count > 0)
            {
                var matched = allRecipes.Where(r => r.Ingredients != null &&
                                                    r.Ingredients.Any(ri => wanted.Any(w => ri.Ingredient.Name.Contains(w,
                                                        StringComparison.InvariantCultureIgnoreCase))))
                    .ToList();

                // Fall back to the whole library if nothing matches, so the button still acts.
                if (matched.Count > 0)
                    pool = matched;
            }

            var rng = new Random();
            var created = new List<MealPlanEntry>();

            // Fetched up front rather than per day: uniqueness spans the whole request, so the
            // days that keep their entries have to be known before the first pick is made.
            var existingInRange = await context.MealPlanEntries
                .Where(m => m.UserId == request.UserId && dates.Contains(m.Date))
                .ToListAsync(cancellationToken);

            if (request.Overwrite)
                context.MealPlanEntries.RemoveRange(existingInRange);

            var keptByDay = request.Overwrite
                ? new Dictionary<DateOnly, int>()
                : existingInRange.GroupBy(m => m.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

            // Recipes already spoken for. Seeded with whatever survives on the days we are not
            // clearing — "something different every day" has to count those too.
            var used = new HashSet<Guid>();
            if (request.UniqueOnly && !request.Overwrite)
                foreach (var entry in existingInRange)
                    used.Add(entry.RecipeId);

            foreach (var date in dates)
            {
                var currentCount = keptByDay.GetValueOrDefault(date);

                var slots = Math.Min(perDay, MealPlanConstants.MaxRecipesPerDay - currentCount);
                if (slots <= 0)
                    continue;

                // Random pick without repeats within a day where the pool allows it.
                var picks = request.UniqueOnly
                    ? PickUnique(pool, used, slots, rng)
                    : pool.OrderBy(_ => rng.Next())
                        .Take(slots)
                        .ToList();

                foreach (var recipe in picks)
                {
                    var entry = new MealPlanEntry
                    {
                        UserId = request.UserId,
                        Date = date,
                        RecipeId = recipe.Id,
                        Recipe = recipe
                    };
                    context.MealPlanEntries.Add(entry);
                    created.Add(entry);
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            return created.ToDtoList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.Common.AddFailed(ErrorMessages.Entities.RandomizedMealPlan));
            return Error.Failure(description: ErrorMessages.Common.AddFailed(ErrorMessages.Entities.RandomizedMealPlan));
        }
    }

    /// <summary>
    /// Takes <paramref name="slots"/> recipes that are not in <paramref name="used"/>, adding
    /// each pick to it so later days cannot repeat them.
    ///
    /// When the pool runs dry — more days than recipes — the set is cleared and the rotation
    /// starts over. Filling the rest of the period with repeats beats leaving it empty, and
    /// the caller asked for variety, not for fewer meals. Terminates because the pool is
    /// known non-empty, so every iteration adds at least one pick.
    /// </summary>
    private static List<Recipe> PickUnique(List<Recipe> pool, HashSet<Guid> used, int slots, Random rng)
    {
        var picks = new List<Recipe>(slots);

        while (picks.Count < slots)
        {
            var candidates = pool.Where(r => !used.Contains(r.Id)).ToList();
            if (candidates.Count == 0)
            {
                used.Clear();
                candidates = pool;
            }

            foreach (var recipe in candidates.OrderBy(_ => rng.Next()).Take(slots - picks.Count))
            {
                picks.Add(recipe);
                used.Add(recipe.Id);
            }
        }

        return picks;
    }
}