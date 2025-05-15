using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InstructorReview: IDeletable
    {
        public int InstructorReviewId { get; set; }
        public int InstructorId { get; set; }
        public int StudentId { get; set; }
        public InstructorUser Instructor { get; set; }
        public StudentUser Student { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
    }
}
