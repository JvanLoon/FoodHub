using ErrorOr;

using FoodCalc.Features.Mapping;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.MealPlan.Commands.RandomizeMealPlan;

public class RandomizeMealPlanCommandHandler(FoodHubDbContext context, ILogger<RandomizeMealPlanCommandHandler> logger)
	: IRequestHandler<RandomizeMealPlanCommand, ErrorOr<List<MealPlanEntryDto>>>
{
	public async Task<ErrorOr<List<MealPlanEntryDto>>> Handle(RandomizeMealPlanCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var dates = request.Dates.Distinct().ToList();
			if (dates.Count == 0)
				return Error.Validation(description: "Select at least one day to randomize.");

			var perDay = Math.Clamp(request.RecipesPerDay, 1, MealPlanConstants.MaxRecipesPerDay);

			var allRecipes = await context.Recipes
				.Include(r => r.Ingredients)
				.ToListAsync(cancellationToken);
			if (allRecipes.Count == 0)
				return Error.Validation(description: "There are no recipes to pick from.");

			// Optional ingredient bias: keep recipes using any requested ingredient.
			var wanted = request.Ingredients
				.Where(i => !string.IsNullOrWhiteSpace(i))
				.Select(i => i.Trim().ToLowerInvariant())
				.Distinct()
				.ToList();

			var pool = allRecipes;
			if (wanted.Count > 0)
			{
				var matched = allRecipes
					.Where(r => r.Ingredients.Any(ri =>
						ri.Name != null && wanted.Any(w => ri.Name.ToLowerInvariant().Contains(w))))
					.ToList();

				// Fall back to the whole library if nothing matches, so the button still acts.
				if (matched.Count > 0)
					pool = matched;
			}

			var rng = new Random();
			var created = new List<MealPlanEntry>();

			foreach (var date in dates)
			{
				var existing = await context.MealPlanEntries
					.Where(m => m.UserId == request.UserId && m.Date == date)
					.ToListAsync(cancellationToken);

				int currentCount;
				if (request.Overwrite && existing.Count > 0)
				{
					context.MealPlanEntries.RemoveRange(existing);
					currentCount = 0;
				}
				else
				{
					currentCount = existing.Count;
				}

				var slots = Math.Min(perDay, MealPlanConstants.MaxRecipesPerDay - currentCount);
				if (slots <= 0)
					continue;

				// Random pick without repeats within a day where the pool allows it.
				var picks = pool.OrderBy(_ => rng.Next()).Take(slots).ToList();
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
			logger.LogError(ex, ErrorMessages.Common.AddFailed("randomized meal plan"));
			return Error.Failure(description: ErrorMessages.Common.AddFailed("randomized meal plan"));
		}
	}
}
