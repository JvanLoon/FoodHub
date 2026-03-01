using ErrorOr;
using MediatR;
using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public class GetAllRecipesQueryHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetAllRecipesQuery, ErrorOr<List<RecipeDto>>>
{
    public Task<ErrorOr<List<RecipeDto>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var recipes = unitOfWork.RecipeRepository.GetAllAsync().ToPagedResult(request.Page, request.PageSize);

			if (!request.WithIngredient)
			{
				foreach (var item in recipes.Items)
				{
					item.RecipeIngredient = null!;
				}
			}

			var result = mapper.Map<List<RecipeDto>>(recipes.Items);
            return Task.FromResult<ErrorOr<List<RecipeDto>>>(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all Recipes");
            return Task.FromResult<ErrorOr<List<RecipeDto>>>(Error.Failure("Failed to get all Recipes"));
        }
    }
}
