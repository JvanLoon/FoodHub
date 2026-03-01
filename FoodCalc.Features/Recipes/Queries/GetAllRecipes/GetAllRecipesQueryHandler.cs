using ErrorOr;
using MediatR;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public class GetAllRecipesQueryHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetAllRecipesQuery, ErrorOr<PagedResultDto<RecipeDto>>>
{
	public Task<ErrorOr<PagedResultDto<RecipeDto>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var query = unitOfWork.RecipeRepository.GetAllAsync();

			if (!string.IsNullOrWhiteSpace(request.Search))
				query = query.Where(r => r.Name.Contains(request.Search));

			var paged = query.ToPagedResult(request.Page, request.PageSize);

			if (!request.WithIngredient)
			{
				foreach (var item in paged.Items)
				{
					item.RecipeIngredient = null!;
				}
			}

			return Task.FromResult<ErrorOr<PagedResultDto<RecipeDto>>>(new PagedResultDto<RecipeDto>
			{
				Items = mapper.Map<List<RecipeDto>>(paged.Items),
				TotalCount = paged.TotalCount,
				Page = paged.Page,
				PageSize = paged.PageSize
			});
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all Recipes");
			return Task.FromResult<ErrorOr<PagedResultDto<RecipeDto>>>(Error.Failure("Failed to get all Recipes"));
		}
	}
}
