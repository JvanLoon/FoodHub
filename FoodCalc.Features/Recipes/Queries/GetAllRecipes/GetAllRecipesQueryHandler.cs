using ErrorOr;
using MediatR;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public class GetAllRecipesQueryHandler(UnitOfWork unitOfWork, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetAllRecipesQuery, ErrorOr<PagedResultDto<RecipeDto>>>
{
	public async Task<ErrorOr<PagedResultDto<RecipeDto>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var query = unitOfWork.RecipeRepository.GetAllAsync();

			if (!string.IsNullOrWhiteSpace(request.Search))
				query = query.Where(r => r.Name.Contains(request.Search));

			var paged = await query.ToPagedResultAsync(request, cancellationToken);

			if (!request.WithIngredient)
			{
				foreach (var item in paged.Items)
				{
					item.RecipeIngredient = null!;
				}
			}

			return new PagedResultDto<RecipeDto>
			{
				Items = paged.Items.ToDtoList(),
				TotalCount = paged.TotalCount,
				Page = paged.Page,
				PageSize = paged.PageSize
			};
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.GetAllFailed("Recipes"));
			return Error.Failure(ErrorMessages.Common.GetAllFailed("Recipes"));
		}
	}
}
