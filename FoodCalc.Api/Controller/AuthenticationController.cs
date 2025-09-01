using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : ControllerBase
{
    [HttpPost("register")]
	[AllowAnonymous]
	public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
		return BadRequest("Register not implemented");
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email};
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok();
    }

    [HttpPost("login")]
	[AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
       var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized("No user found");

		if (!user.EmailConfirmed)
			return Unauthorized("Email not confirmed");

		if (user.LockoutEnabled)
		{
			if (user.LockoutEnd < DateTime.Now)
			{
				await userManager.SetLockoutEnabledAsync(user, false);
			}
			else
				return Unauthorized("User Lockedout");
		}

		var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
		{
			await userManager.AccessFailedAsync(user);
			return Unauthorized("Invalid password");
		}

		List<Claim> claims = [];

		claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
		claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));

		var roles = await userManager.GetRolesAsync(user);
		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds
        );
        var response = new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email,
        };
        return Ok(response);
    }

	[HttpPost("toggleUser")]
	[Authorize("Admin")]
	public async Task<IActionResult> EnableUser([FromQuery] string email, [FromQuery] bool enable = true)
	{
		var user = await userManager.FindByEmailAsync(email);

		if (user == null)
			return NotFound("User not found");

		//enable user
		user.EmailConfirmed = enable;

		//check if user has role user, of not add it
		if (!await userManager.IsInRoleAsync(user, "User"))
		{
			await userManager.AddToRoleAsync(user, "User");
		}


		var result = await userManager.UpdateAsync(user);

		if (!result.Succeeded)
			return BadRequest(result.Errors);

		return Ok();
	}

	[HttpPost("checkjwttoken")]
	[AllowAnonymous]
	public async Task<IActionResult> CheckJWTToken([FromQuery] string token)
	{
		if (string.IsNullOrWhiteSpace(token))
			return Ok(false);

		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
		try
		{
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = configuration["Jwt:Issuer"],
				ValidateAudience = true,
				ValidAudience = configuration["Jwt:Audience"],
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuerSigningKey = true,
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			return Ok(true);
		}
		catch
		{
			return Ok(false);
		}
	}
}
