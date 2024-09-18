using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AchievementTracker.Models;

namespace AchievementTracker.Repositories
{
    public class AchievementRepository
    {
        private readonly string _connectionString;

        public AchievementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Achievement> GetAchievements(int userId)
        {
            var achievements = new List<Achievement>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Achievements WHERE UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        achievements.Add(new Achievement
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            Date = reader.GetDateTime(3),
                            Tag = reader.GetString(4),
                            UserId = reader.GetInt32(5) // Ensure UserId is included
                        });
                    }
                }
            }

            return achievements;
        }

        public void AddAchievement(Achievement achievement)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Achievements (Title, Description, Date, Tag, UserId) VALUES (@Title, @Description, @Date, @Tag, @UserId)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", achievement.Title);
                    cmd.Parameters.AddWithValue("@Description", achievement.Description);
                    cmd.Parameters.AddWithValue("@Date", achievement.Date);
                    cmd.Parameters.AddWithValue("@Tag", achievement.Tag);
                    cmd.Parameters.AddWithValue("@UserId", achievement.UserId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAchievement(Achievement achievement)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Achievements SET Title = @Title, Description = @Description, Date = @Date, Tag = @Tag WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", achievement.Id);
                    cmd.Parameters.AddWithValue("@Title", achievement.Title);
                    cmd.Parameters.AddWithValue("@Description", achievement.Description);
                    cmd.Parameters.AddWithValue("@Date", achievement.Date);
                    cmd.Parameters.AddWithValue("@Tag", achievement.Tag);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAchievement(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Achievements WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
