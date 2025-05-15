using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CourseReview
    {
        public int CourseReviewId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public StudentUser Student { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
