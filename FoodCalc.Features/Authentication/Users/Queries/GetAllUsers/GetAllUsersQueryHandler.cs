using AutoMapper;
using ErrorOr;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;
public class GetAllUsersQueryHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllUsersQueryHandler> logger, UserManager<IdentityUser> userManager) : IRequestHandler<GetAllUsersQuery, ErrorOr<List<UserDto>>>
{
    public async Task<ErrorOr<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = unitOfWork.UserRepository.GetAllAsync().ToPagedResult(request.Page, request.PageSize);
            var userDtos = new List<UserDto>();
            foreach (var user in users.Items)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userDto = mapper.Map<UserDto>(user);
                userDto.Enabled = user.EmailConfirmed;
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
