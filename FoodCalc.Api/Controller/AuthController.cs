using Microsoft.AspNetCore.Mvc;
using FoodHub.Persistence;
using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FoodCalc.Api.Controller
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized();
            // TODO: Issue JWT or session
            return Ok();
        }

        private bool VerifyPassword(string password, string hash)
        {
            // Simple SHA256 hash check (replace with proper hashing in production)
            using var sha = SHA256.Create();
            var hashed = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return hashed == hash;
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
