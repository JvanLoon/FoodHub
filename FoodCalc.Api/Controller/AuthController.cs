using FoodHub.Persistence;
using FoodHub.Persistence.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Api.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController(SignInManager<User> signInManager, UserManager<User> userManager) : ControllerBase
{

	[HttpGet("admin/users")]
	public async Task<IActionResult> GetUsers()
	{
		var users = userManager.Users.Select(u => new { id = u.Id, email = u.Email, enabled = u.Enabled }).ToList();
		return Ok(users);
	}

	[HttpPost("admin/enable/{id}")]
	public async Task<IActionResult> EnableUser(string id)
	{
		var user = await userManager.FindByIdAsync(id);
		if (user == null) return NotFound();
		user.Enabled = true;
		await userManager.UpdateAsync(user);
		return Ok();
	}


	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request)
	{
		var user = new User { UserName = request.Email, Email = request.Email };
		var result = await userManager.CreateAsync(user, request.Password);
		if (result.Succeeded)
		{
			// Optionally add to role: await userManager.AddToRoleAsync(user, "Admin");
			return Ok();
		}
		return BadRequest(result.Errors);
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		var user = await userManager.FindByEmailAsync(request.Email);
		if (user == null || !user.Enabled)
			return Unauthorized();
		var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
		if (!result.Succeeded)
			return Unauthorized();
		// TODO: Issue JWT or session
		return Ok();
	}
}

public class LoginRequest
{
	public string Email { get; set; }
	public string Password { get; set; }
}

public class RegisterRequest
{
	public string Email { get; set; }
	public string Password { get; set; }
}
