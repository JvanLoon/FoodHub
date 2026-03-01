using AutoMapper;
using ErrorOr;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;
public class GetAllUsersQueryHandler(UnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllUsersQueryHandler> logger, UserManager<IdentityUser> userManager) : IRequestHandler<GetAllUsersQuery, ErrorOr<PagedResultDto<UserDto>>>
{
    public async Task<ErrorOr<PagedResultDto<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = unitOfWork.UserRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(u => u.Email!.Contains(request.Search));

            var paged = query.ToPagedResult(request.Page, request.PageSize);

            var userDtos = new List<UserDto>();
            foreach (var user in paged.Items)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userDto = mapper.Map<UserDto>(user);
                userDto.Enabled = user.EmailConfirmed;
                userDto.Roles = roles.ToList();
                userDtos.Add(userDto);
            }

            return new PagedResultDto<UserDto>
            {
                Items = userDtos,
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get all Users");
            return Error.Failure("Failed to get all Users");
        }
    }
}
