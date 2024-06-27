namespace coding_tracker.Models
{
    public class CodingTracker
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
        
        public TimeSpan Duration { get; set; }
    }
}