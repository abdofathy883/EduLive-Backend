using Core.Enums;
using Core.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Lesson: IAuditable, IDeletable
    {
        public int LessonId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public LessonPlatform LessonPlatform { get; set; }

        public int? GoogleMeetId { get; set; }
        public GoogleMeetLesson GoogleMeetLesson { get; set; }

        public int? ZoomMeetingId { get; set; }
        public ZoomMeeting ZoomMeeting { get; set; }

        public virtual int CourseId { get; set; }
        public Course Course { get; set; }

        public virtual string InstructorId { get; set; }
        public InstructorUser Instructor { get; set; }

        public virtual string StudentId { get; set; }
        public StudentUser Student {  get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
