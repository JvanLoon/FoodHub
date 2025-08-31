using ErrorOr;

using FoodCalc.Features.Recipes.Queries.GetAllRecipes;
using FoodCalc.Features.Users.Commands.GetUserByEmail;

using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize("Admin,Moderator,User")]
public class AdminController(IMediator mediator) : ControllerBase
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
}
