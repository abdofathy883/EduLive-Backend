using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Certificate
    {
        public required string SerialNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public float Score { get; set; }
        public List<Course> Courses { get; set; }
        public List<StudentUser> StudentUsers { get; set; } = new List<StudentUser>();
    }
}
