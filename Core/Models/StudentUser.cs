using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class StudentUser : BaseUser
    {
        public int StudentId { get; set; }
        [ForeignKey("Certificate")]
        public virtual string? CertificateSerialNumber { get; set; }
        public List<Course>? EnrolledCourses { get; set; }
        public List<Lesson>? EnrolledLessons { get; set; }
        public List<CourseReview>? CourseReviews { get; set; }
        public List<InstructorReview>? InstructorReviews { get; set; }
        public virtual List<Payment> Payments { get; set; } = new List<Payment>();
        public virtual List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        //public List<Notification> Notifications { get; set; }
    }
}
