using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InstructorReview: IAuditable, IDeletable
    {
        public int InstructorReviewId { get; set; }
        public string InstructorId { get; set; }
        public InstructorUser Instructor { get; set; }
        public string StudentId { get; set; }
        public StudentUser Student { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
