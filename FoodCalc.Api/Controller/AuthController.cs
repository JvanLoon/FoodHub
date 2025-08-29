using Microsoft.AspNetCore.Mvc;
using FoodHub.Persistence.Entities;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FoodCalc.Api.Controller
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration) : ControllerBase
	{
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var user = await userManager.FindByEmailAsync(request.Email);
			if (user == null || !user.Enabled)
				return Unauthorized();
			var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
			if (!result.Succeeded)
				return Unauthorized();

			// Genereer JWT-token
			var tokenHandler = new JwtSecurityTokenHandler();
			var raw_key = configuration["Jwt:Key"];
			if(string.IsNullOrEmpty(raw_key)){
				// Handle missing key, app did not load configuration properly
				return Unauthorized();
			}
			var key = Encoding.UTF8.GetBytes(raw_key);
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
				new Claim("enabled", user.Enabled.ToString()),
				new Claim(ClaimTypes.Name, user.UserName ?? "")
			};
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddHours(12),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var jwt = tokenHandler.WriteToken(token);

			// Zet JWT als HttpOnly cookie
			Response.Cookies.Append("jwt", jwt, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddHours(12)
			});

			return Ok(new { token = jwt });
		}

		[HttpPost("logout")]
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return Ok();
		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var user = new User { UserName = request.Email, Email = request.Email, Enabled = false };
			var result = await userManager.CreateAsync(user, request.Password);
			if (result.Succeeded)
			{
				// Optionally add to role: await userManager.AddToRoleAsync(user, "Admin");
				return Ok();
			}
			return BadRequest(result.Errors);
		}

		[HttpGet("admin/users")]
		public IActionResult GetUsers()
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

		// Request classes en helpers onderaan:
		public class LoginRequest
		{
			public string? Email { get; set; }
			public string? Password { get; set; }
		}

		public class RegisterRequest
		{
			public string? Email { get; set; }
			public string? Password { get; set; }
		}
	}
}
