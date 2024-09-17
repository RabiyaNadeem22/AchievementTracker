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

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            _repository.AddUser(user);
            return Ok();
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
