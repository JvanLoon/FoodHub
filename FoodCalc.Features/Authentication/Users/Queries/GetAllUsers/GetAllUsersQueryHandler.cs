using ErrorOr;
using FoodCalc.Features.Authentication.Presence;
using FoodCalc.Features.Mapping;
using FoodHub.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(
    FoodHubDbContext context,
    ILogger<GetAllUsersQueryHandler> logger,
    UserManager<IdentityUser> userManager) : IRequestHandler<GetAllUsersQuery, ErrorOr<PagedResultDto<UserDto>>>
{
    public async Task<ErrorOr<PagedResultDto<UserDto>>> Handle(GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(u => u.Email!.Contains(request.Search));

            var paged = await query.ToPagedResultAsync(request, cancellationToken);

            // One presence lookup for the whole page, keyed by id, instead of a query per row.
            var userIds = paged.Items.Select(u => u.Id)
                .ToList();

            var presence = await context.UserPresences.Where(p => userIds.Contains(p.UserId))
                .ToDictionaryAsync(p => p.UserId, cancellationToken);

            // Captured once so every row on the page is judged against the same instant.
            var now = DateTime.UtcNow;

            var userDtos = new List<UserDto>();
            foreach (var user in paged.Items)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userDto = user.ToUserDto();
                userDto.Enabled = user.EmailConfirmed;
                userDto.Roles = roles.ToList();

                var seen = presence.GetValueOrDefault(user.Id);
                userDto.IsOnline = PresenceWindow.IsOnline(seen, now);
                userDto.LastSeenUtc = seen?.LastSeenUtc;

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
            logger.LogError(ex, ErrorMessages.Common.GetAllFailed(ErrorMessages.Entities.Users));
            return Error.Failure(description: ErrorMessages.Common.GetAllFailed(ErrorMessages.Entities.Users));
        }
    }
}