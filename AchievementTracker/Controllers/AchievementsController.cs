using AchievementTracker.Models;
using AchievementTracker.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/achievements
        [HttpGet]
        public IActionResult GetAchievements()
        {
            try
            {
                var achievements = _repository.GetAchievements();
                return Ok(achievements);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/achievements/{id}
        [HttpGet("{id}")]
        public IActionResult GetAchievement(int id)
        {
            try
            {
                var achievements = _repository.GetAchievements();
                var achievement = achievements.FirstOrDefault(a => a.Id == id);

                if (achievement == null)
                {
                    return NotFound();
                }

                return Ok(achievement);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/achievements
        [HttpPost]
        public IActionResult CreateAchievement([FromBody] Achievement achievement)
        {
            if (achievement == null)
            {
                return BadRequest("Achievement is null");
            }

            try
            {
                _repository.AddAchievement(achievement);
                return CreatedAtAction(nameof(GetAchievement), new { id = achievement.Id }, achievement);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/achievements/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateAchievement(int id, [FromBody] Achievement achievement)
        {
            if (achievement == null || id != achievement.Id)
            {
                return BadRequest("Achievement is null or id mismatch");
            }

            try
            {
                var existingAchievements = _repository.GetAchievements();
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
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/achievements/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteAchievement(int id)
        {
            try
            {
                var existingAchievements = _repository.GetAchievements();
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
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
