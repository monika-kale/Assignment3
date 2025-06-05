using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPut]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid user data.");
            }

            // Check if the user already exists
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(RegisterUser), new { id = user.Id }, user);
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
            if (user == null) return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
          
        }
    }
}
