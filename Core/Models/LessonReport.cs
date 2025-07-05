using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class LessonReport: IDeletable
    {
        public int Id { get; set; }
        public string MemorizedContent { get; set; }
        public string ExplainedTopics { get; set; }
        public string DiscussedValues { get; set; }
        public string StudentId { get; set; }
        public StudentUser Student { get; set; }
        public string InstructorId { get; set; }
        public InstructorUser Instructor { get; set; }
        public bool IsDeleted { get; set; }
    }
}
