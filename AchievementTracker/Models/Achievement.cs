namespace AchievementTracker.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Tag { get; set; }
        public int UserId { get; set; }  // Foreign key reference
    }
}
