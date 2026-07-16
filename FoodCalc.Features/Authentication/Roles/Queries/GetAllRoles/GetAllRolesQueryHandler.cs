using ErrorOr;

using FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;
using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;

using FoodHub.DTOs;
using FoodHub.Persistence.Persistence;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;
public class GetAllRolesQueryHandler(UnitOfWork unitOfWork, ILogger<GetAllUsersQueryHandler> logger) : IRequestHandler<GetAllRolesQuery, ErrorOr<PagedResultDto<string>>>
{
	public async Task<ErrorOr<PagedResultDto<string>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var query = unitOfWork.RoleRepository.GetAllAsync();

			if (!string.IsNullOrWhiteSpace(request.Search))
				query = query.Where(r => r.Contains(request.Search));

			return await query.ToPagedResultAsync(request, cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, ErrorMessages.Common.GetAllFailed("Roles"));
			return Error.Failure(ErrorMessages.Common.GetAllFailed("Roles"));
		}
	}
}
