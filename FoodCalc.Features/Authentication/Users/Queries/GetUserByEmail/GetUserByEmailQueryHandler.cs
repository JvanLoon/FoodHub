using AutoMapper;

using ErrorOr;

using FoodCalc.Features.Recipes.Queries.GetAllRecipes;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Authentication.Users.Queries.GetUserByEmail;
public class GetUserByEmailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetUserByEmailQuery, ErrorOr<UserDto>>
{
	public async Task<ErrorOr<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var user = await unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);

			return mapper.Map<UserDto>(user);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get User by email");
			//return message because GetByEmailAsync returns a error with message
			return Error.Failure(ex.Message);
		}
	}
}
