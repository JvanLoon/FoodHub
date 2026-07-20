using ErrorOr;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;
public class GetAllUsersQueryHandler(FoodHubDbContext context, ILogger<GetAllUsersQueryHandler> logger, UserManager<IdentityUser> userManager) : IRequestHandler<GetAllUsersQuery, ErrorOr<PagedResultDto<UserDto>>>
{
    public async Task<ErrorOr<PagedResultDto<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(u => u.Email!.Contains(request.Search));

            var paged = await query.ToPagedResultAsync(request, cancellationToken);

            var userDtos = new List<UserDto>();
            foreach (var user in paged.Items)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userDto = user.ToUserDto();
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
            logger.LogError(ex, ErrorMessages.Common.GetAllFailed("Users"));
            return Error.Failure(ErrorMessages.Common.GetAllFailed("Users"));
        }
    }
}
