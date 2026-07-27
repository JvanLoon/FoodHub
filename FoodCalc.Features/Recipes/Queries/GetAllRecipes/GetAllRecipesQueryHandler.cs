using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;

public class GetAllRecipesQueryHandler(FoodHubDbContext context, ILogger<GetAllRecipesQueryHandler> logger)
	: IRequestHandler<GetAllRecipesQuery, ErrorOr<PagedResultDto<RecipeDto>>>
{
	public async Task<ErrorOr<PagedResultDto<RecipeDto>>> Handle(GetAllRecipesQuery request,
																 CancellationToken cancellationToken
	)
	{
		try
		{
			var query = context.Recipes.AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.Search))
				query = query.Where(r => r.Name != null && r.Name.Contains(request.Search));

			var paged = await query.ToPagedResultAsync(request, cancellationToken);

			var items = paged.Items.ToDtoList();

			// Callers that only need names (the calendar picker) ask for recipes without
			// ingredients. Strip them off the DTO — never off the tracked entity, which
			// would both break the mapping below and dirty the change tracker.
			if (!request.WithIngredient)
			{
				foreach (var item in items) { item.Ingredients = null!; }
			}

			return new PagedResultDto<RecipeDto>
			{
				Items = items,
				TotalCount = paged.TotalCount,
				Page = paged.Page,
				PageSize = paged.PageSize
			};
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.GetAllFailed("Recipes"));
			return Error.Failure(description: ErrorMessages.Common.GetAllFailed("Recipes"));
		}
	}
}