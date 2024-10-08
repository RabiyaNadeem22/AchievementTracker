﻿using Microsoft.Data.SqlClient;
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
                string query = "UPDATE Achievements SET Title = @Title, Description = @Description, Date = @Date, Tag = @Tag WHERE Id = @Id AND UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", achievement.Id);
                    cmd.Parameters.AddWithValue("@UserId", achievement.UserId); // Ensure UserId is included for authorization
                    cmd.Parameters.AddWithValue("@Title", achievement.Title);
                    cmd.Parameters.AddWithValue("@Description", achievement.Description);
                    cmd.Parameters.AddWithValue("@Date", achievement.Date);
                    cmd.Parameters.AddWithValue("@Tag", achievement.Tag);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public async Task<List<Achievement>> GetAchievementsByTagAsync(string tag, int userId)
        {
            var achievements = new List<Achievement>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT Id, Title, Description, Date, Tag, UserId FROM Achievements WHERE Tag LIKE @Tag AND UserId = @UserId", connection))
                {
                    command.Parameters.Add(new SqlParameter("@Tag", $"%{tag}%"));
                    command.Parameters.Add(new SqlParameter("@UserId", userId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            achievements.Add(new Achievement
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                Date = reader.GetDateTime(3),
                                Tag = reader.GetString(4),
                                UserId = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }

            return achievements;
        }

        public void DeleteAchievement(int userId, int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Achievements WHERE Id = @Id AND UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("No rows were deleted. Achievement might not exist or does not belong to the user.");
                    }
                }
            }
        }
    }
    }
