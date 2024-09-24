using AchievementTracker.Models;
using AchievementTracker.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AchievementTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly AchievementRepository _repository;
        private readonly ILogger<AchievementsController> _logger;

        public AchievementsController(AchievementRepository repository, ILogger<AchievementsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetAchievements(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            try
            {
                var achievements = _repository.GetAchievements(userId);
                if (achievements == null || !achievements.Any())
                {
                    return NotFound("No achievements found for this user.");
                }

                return Ok(achievements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving achievements for user ID {UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]

        public IActionResult GetAchievement(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0; // Obtain the user ID from session or context
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
                _logger.LogError(ex, "An error occurred while retrieving achievement ID {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CreateAchievement([FromBody] Achievement achievement, int userId)
        {
            if (achievement == null)
            {
                return BadRequest("Achievement is null");
            }

            try
            {
                achievement.UserId = userId; // Set UserId from the parameter
                _repository.AddAchievement(achievement);
                return CreatedAtAction(nameof(GetAchievement), new { id = achievement.Id }, achievement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an achievement for user ID {UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("{userId}/{id}")]
        public IActionResult UpdateAchievement(int userId, int id, [FromBody] Achievement achievement)
        {
            if (achievement == null || id != achievement.Id)
            {
                return BadRequest("Achievement is null or id mismatch");
            }

            try
            {
                var existingAchievement = _repository.GetAchievements(userId)
                    .FirstOrDefault(a => a.Id == id);

                if (existingAchievement == null)
                {
                    return NotFound("Achievement not found.");
                }

                // Check if the UserId matches
                if (existingAchievement.UserId != userId)
                {
                    return Forbid("You are not authorized to update this achievement.");
                }

                // Update the achievement
                _repository.UpdateAchievement(achievement);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating achievement ID {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<List<Achievement>>> SearchAchievements([FromQuery] string tag, [FromQuery] int userId)
        {
            // Check if userId is valid
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var achievements = await _repository.GetAchievementsByTagAsync(tag, userId);

            if (achievements.Count == 0)
            {
                return NotFound("No achievements found with the given tag.");
            }

            return Ok(achievements);
        }

        [HttpDelete("{userid}/{id}")]
        public IActionResult DeleteAchievement(int userid, int id)
        {
            try
            {
                var existingAchievement = _repository.GetAchievements(userid)
                    .FirstOrDefault(a => a.Id == id);

                if (existingAchievement == null)
                {
                    return NotFound("Achievement not found or you do not have permission to delete it.");
                }

                // Pass userId and id to the repository method
                _repository.DeleteAchievement(userid, id);
                return NoContent();
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error occurred while deleting achievement ID {Id}", id);
                return StatusCode(500, "Database error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting achievement ID {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}