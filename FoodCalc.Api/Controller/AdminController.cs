using ErrorOr;

using FoodCalc.Features.Authentication.Roles.Queries.GetAllRoles;
using FoodCalc.Features.Authentication.Users.Queries.GetAllUsers;
using FoodCalc.Features.Recipes.Queries.GetAllRecipes;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Linq;

[ApiController]
[Route("api/[controller]")]
[Authorize("Admin,Moderator")]
public class AdminController(IMediator mediator, UserManager<IdentityUser> userManager) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
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

	[HttpGet("userroles")]
	public async Task<IActionResult> GetUserRoles([FromQuery] string email)
	{
		var user = await userManager.FindByEmailAsync(email);
		if (user == null)
			return Unauthorized("No user found");

		return Ok(await userManager.GetRolesAsync(user));
	}

	[HttpPost("userroles")]
	public async Task<IActionResult> AddUserRole([FromQuery] string email, [FromQuery] string role)
	{
		var user = await userManager.FindByEmailAsync(email);
		if (user == null)
			return NotFound("No user found");

		var result = await userManager.AddToRoleAsync(user, role);
		if (!result.Succeeded)
			return Problem(detail: string.Join(", ", result.Errors.Select(e => e.Description)));

		return Ok();
	}

	[HttpDelete("userroles")]
	public async Task<IActionResult> RemoveUserRole([FromQuery] string email, [FromQuery] string role)
	{
		var user = await userManager.FindByEmailAsync(email);
		if (user == null)
			return NotFound("No user found");

		var result = await userManager.RemoveFromRoleAsync(user, role);
		if (!result.Succeeded)
			return Problem(detail: string.Join(", ", result.Errors.Select(e => e.Description)));

		return Ok();
	}
}
