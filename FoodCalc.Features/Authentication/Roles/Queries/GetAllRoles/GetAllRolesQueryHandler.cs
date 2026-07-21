using ErrorOr;

using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;

using FoodHub.DTOs;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;
public class GetAllRolesQueryHandler(FoodHubDbContext context, ILogger<GetAllUsersQueryHandler> logger) : IRequestHandler<GetAllRolesQuery, ErrorOr<PagedResultDto<string>>>
{
	public async Task<ErrorOr<PagedResultDto<string>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var query = context.Roles.Select(r => r.Name!).AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.Search))
				query = query.Where(r => r.Contains(request.Search));

			return await query.ToPagedResultAsync(request, cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.GetAllFailed("Roles"));
			return Error.Failure(description: ErrorMessages.Common.GetAllFailed("Roles"));
		}
	}
}
