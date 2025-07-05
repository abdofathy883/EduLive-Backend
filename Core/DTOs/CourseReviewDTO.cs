using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CourseReviewDTO
    {
        public string StudentId { get; set; }
        public int CourseId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
