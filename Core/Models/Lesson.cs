using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Lesson: IAuditable, IDeletable
    {
        public int LessonId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int Duration { get; set; }
        public DateTime Date_Time { get; set; }
        public LessonPlatform LessonPlatform { get; set; }
        //Google Meet Properties
        public string? GoogleMeetURL { get; set; }
        //Zoom Properties
        public string? ZoomMeetingId { get; set; }
        public string? ZoomJoinURL { get; set; }
        public string? ZoomStartUrl { get; set; }
        public string? ZoomPassword { get; set; }
        // Navigation Properties
        //Course
        [ForeignKey("Course")]
        public virtual int CourseId { get; set; }
        public Course Course { get; set; }
        //Instructor
        [ForeignKey("InstructorUser")]
        public virtual string InstructorId { get; set; }
        public InstructorUser Instructor { get; set; }
        //Student
        [ForeignKey("StudentUser")]
        public virtual string StudentId { get; set; }
        public StudentUser Student {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
