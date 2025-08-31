using FoodHub.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AdminController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users.Select(u => new UserDto
        {
            Email = u.Email,
            Enabled = u.Enabled
        }).ToList();
        return Ok(users);
    }

    public class UserDto
    {
        public string Email { get; set; }
        public bool Enabled { get; set; }
    }
}
