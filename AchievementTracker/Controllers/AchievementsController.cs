using AchievementTracker.Models;
using AchievementTracker.Repositories;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPut("{id}")]
        public IActionResult UpdateAchievement(int id, [FromBody] Achievement achievement)
        {
            if (achievement == null || id != achievement.Id)
            {
                return BadRequest("Achievement is null or id mismatch");
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var existingAchievement = _repository.GetAchievements(userId)
                    .FirstOrDefault(a => a.Id == id);

                if (existingAchievement == null)
                {
                    return NotFound();
                }

                // Check if the UserId matches
                if (existingAchievement.UserId != userId)
                {
                    return Forbid("You are not authorized to update this achievement.");
                }

                _repository.UpdateAchievement(achievement);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating achievement ID {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAchievement(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var existingAchievement = _repository.GetAchievements(userId)
                    .FirstOrDefault(a => a.Id == id);

                if (existingAchievement == null)
                {
                    return NotFound("Achievement not found or you do not have permission to delete it.");
                }

                _repository.DeleteAchievement(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting achievement ID {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
    }
