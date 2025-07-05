using Core.Interfaces;

namespace Core.Models
{
    public class ZoomMeeting: IAuditable, IDeletable
    {
        public int Id { get; set; }
        //public string Topic { get; set; }
        public string ZoomMeetingId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; } // in minutes
        public string JoinUrl { get; set; }
        public string StartUrl { get; set; }
        public string Password { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
