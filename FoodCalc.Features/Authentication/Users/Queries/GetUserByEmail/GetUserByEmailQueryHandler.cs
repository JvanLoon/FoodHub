using ErrorOr;

using FoodCalc.Features.Mapping;
using FoodCalc.Features.Recipes.Queries.GetAllRecipes;

using FoodHub.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Authentication.Users.Queries.GetUserByEmail;
public class GetUserByEmailQueryHandler(FoodHubDbContext context, ILogger<GetAllRecipesQueryHandler> logger) : IRequestHandler<GetUserByEmailQuery, ErrorOr<UserDto>>
{
	public async Task<ErrorOr<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var user = await context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

			if (user is null)
				return Error.Failure(description: ErrorMessages.Common.NotFound("User"));

			return user.ToUserDto();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.User.GetByEmailFailed);
			return Error.Failure(description: ErrorMessages.User.GetByEmailFailed);
		}
	}
}
