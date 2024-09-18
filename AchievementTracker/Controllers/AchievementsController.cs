using AchievementTracker.Models;
using AchievementTracker.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AchievementTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly AchievementRepository _repository;

        public AchievementsController(AchievementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAchievements([FromQuery] int userId)
        {
            try
            {
                var achievements = _repository.GetAchievements(userId);
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAchievement(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;// Obtain the user ID from session or context
                var achievements = _repository.GetAchievements(userId);
                var achievement = achievements.FirstOrDefault(a => a.Id == id);

                if (achievement == null)
                {
                    return NotFound();
                }

                return Ok(achievement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CreateAchievement([FromBody] Achievement achievement)
        {
            if (achievement == null)
            {
                return BadRequest("Achievement is null");
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;// Obtain the user ID from session or context
                achievement.UserId = userId;
                _repository.AddAchievement(achievement);
                return CreatedAtAction(nameof(GetAchievement), new { id = achievement.Id }, achievement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAchievement(int id, [FromBody] Achievement achievement)
        {
            if (achievement == null || id != achievement.Id)
            {
                return BadRequest("Achievement is null or id mismatch");
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;// Obtain the user ID from session or context
                var existingAchievements = _repository.GetAchievements(userId);
                var existingAchievement = existingAchievements.FirstOrDefault(a => a.Id == id);

                if (existingAchievement == null)
                {
                    return NotFound();
                }

                _repository.UpdateAchievement(achievement);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAchievement(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var existingAchievements = _repository.GetAchievements(userId);
                var existingAchievement = existingAchievements.FirstOrDefault(a => a.Id == id);

                if (existingAchievement == null)
                {
                    return NotFound();
                }

                _repository.DeleteAchievement(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
