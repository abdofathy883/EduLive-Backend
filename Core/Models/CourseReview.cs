using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CourseReview: IAuditable
    {
        public int CourseReviewId { get; set; }
        public string StudentId { get; set; }
        public StudentUser Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
