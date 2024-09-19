using AchievementTracker.Models;
using AchievementTracker.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AchievementTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _repository;

        public UsersController(UserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_repository.GetUsers());
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] User user)
        {
            if (_repository.GetUserByEmail(user.Email) != null)
            {
                return Conflict(new { message = "Email already exists" });
            }

            _repository.AddUser(user);
            return Ok(new { message = "User created successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = _repository.GetUserByEmail(loginModel.Email);

            // Check if the user exists and the password matches
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Return userId and success message
            return Ok(new { message = "Login successful", userId = user.Id });
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();
            _repository.UpdateUser(user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            _repository.DeleteUser(id);
            return Ok();
        }
    }
}
