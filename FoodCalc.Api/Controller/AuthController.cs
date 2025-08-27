using Microsoft.AspNetCore.Mvc;
using FoodHub.Persistence;
using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FoodCalc.Api.Controller
    [HttpGet("admin/users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users.Select(u => new { id = u.Id, email = u.Email, enabled = u.Enabled }).ToList();
        return Ok(users);
    }

    [HttpPost("admin/enable/{id}")]
    public async Task<IActionResult> EnableUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        user.Enabled = true;
        await _userManager.UpdateAsync(user);
        return Ok();
    }
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new User { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            // Optionally add to role: await _userManager.AddToRoleAsync(user, "Admin");
            return Ok();
        }
        return BadRequest(result.Errors);
    }
{
    [ApiController]
    [Route("api/auth")]
    using Microsoft.AspNetCore.Identity;
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.Enabled)
                return Unauthorized();
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
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
}
