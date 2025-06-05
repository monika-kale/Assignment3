using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data; // Make sure this is included
using Microsoft.EntityFrameworkCore;
using AutoMapper;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public UserController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id); // 👈 Fix here
            if (user == null) return NotFound();
            return Ok(user);
        }

        ///get all users list
        // [HttpGet]
        // public async Task<IActionResult> GetAllUser()
        // {
        //     var user = await _db.Users.ToListAsync();
        //     if (user == null) return NotFound();
        //     return Ok(user);
        // }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        /// update user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
                return BadRequest("User ID mismatch");

            var existingUser = await _db.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            existingUser.Username = updatedUser.Username;
            existingUser.Email = updatedUser.Email;
            existingUser.Password = updatedUser.Password;

            _db.Users.Update(existingUser);
            await _db.SaveChangesAsync();

            return Ok(existingUser);
        }

        ///delete user 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users.ToListAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);
        }


        [HttpGet("error-test")]
        public IActionResult GetError()
        {
            throw new Exception("Test exception");
        }
    }
}
