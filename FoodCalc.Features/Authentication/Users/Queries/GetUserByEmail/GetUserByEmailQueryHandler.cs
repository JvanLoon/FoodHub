using ErrorOr;

using FoodCalc.Features.Mapping;
using FoodCalc.Features.Recipes.Queries.GetAllRecipes;

using FoodHub.DTOs;
using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Authentication.Users.Queries.GetUserByEmail;
public class GetUserByEmailQueryHandler(UnitOfWork unitOfWork, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetUserByEmailQuery, ErrorOr<UserDto>>
{
	public async Task<ErrorOr<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var user = await unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);

			if (user is null)
				return Error.Failure(ErrorMessages.User.NotFound);

			return user.ToUserDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.User.GetByEmailFailed);
			//return message because GetByEmailAsync returns a error with message
			return Error.Failure(ex.Message);
		}
	}
}
