using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Lesson
    {
        public int CourseId { get; set; }
        public int LessonId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int Duration { get; set; }
        public DateTime Date_Time { get; set; }
        public LessonPlatform LessonPlatform { get; set; }
        public string? GoogleMeetURL { get; set; }
        public string? ZoomURL { get; set; }
        [ForeignKey("InstructorUser")]
        public virtual int InstructorId { get; set; }
        [ForeignKey("StudentUser")]
        public virtual int StudentId { get; set; }
        public Course Course { get; set; }
        public InstructorUser Instructor { get; set; }
        public StudentUser Student {  get; set; }
    }
}
