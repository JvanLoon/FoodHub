using AutoMapper;

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
public class GetAllRolesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllUsersQueryHandler> logger) : IRequestHandler<GetAllRolesQuery, ErrorOr<List<string>>>
{
	public async Task<ErrorOr<List<string>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var roles = await unitOfWork.RoleRepository.GetAllAsync(cancellationToken);

			return roles;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all Users");
			return Error.Failure("Failed to get all Users");
		}
	}
}
