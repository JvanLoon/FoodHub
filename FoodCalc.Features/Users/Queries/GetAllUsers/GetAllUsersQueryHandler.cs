using AutoMapper;

using ErrorOr;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Recipes.Queries.GetAllRecipes;
public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllUsersQueryHandler> logger, UserManager<User> userManager) : IRequestHandler<GetAllUsersQuery, ErrorOr<List<UserDto>>>
{
	public async Task<ErrorOr<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
	{
		try
		{
			List<User> users = await unitOfWork.UserRepository.GetAllAsync(cancellationToken);

			var userDtos = new List<UserDto>();
			foreach (var user in users)
			{
				var roles = await userManager.GetRolesAsync(user);
				var userDto = mapper.Map<UserDto>(user);
				userDto.Roles = roles.ToList();
				userDtos.Add(userDto);
			}

			return userDtos;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all Users");
			return Error.Failure("Failed to get all Users");
		}
	}
}
