namespace Core.Models
{
    public class WhatsappALert
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string UserId { get; set; }
        public DateTime SentAt { get; set; }
    }
}
