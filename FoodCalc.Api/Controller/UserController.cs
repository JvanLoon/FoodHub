using ErrorOr;

using FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;
using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;

using FoodHub.DTOs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodCalc.Api.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize("Admin,Moderator,User")]
public class UserController(IMediator mediator, UserManager<IdentityUser> userManager) : ControllerBase
{
	[HttpGet("users")]
	public async Task<IActionResult> GetUsers()
	{
		//BlackListDto

		ErrorOr<List<UserDto>> result = await mediator.Send(new GetAllUsersQuery());

		return result.Match(
			userList =>
			{
				var users = userList.Select(u => new UserDto
				{
					Email = u.Email,
					Enabled = u.Enabled,
					Roles = u.Roles
				}).ToList();
				return Ok(users);
			},
			errors => Problem(detail: string.Join(", ", errors.Select(e => e.ToString())))
		);
	}

	[HttpGet("allroles")]
	public async Task<IActionResult> GetAllUserRoles()
	{
		ErrorOr<List<string>> result = await mediator.Send(new GetAllRolesQuery());

		return result.Match(
			userList => Ok(string.Join(",", result.Value)),
			errors => Problem(detail: string.Join(", ", errors.Select(e => e.ToString())))
		);
	}
}
