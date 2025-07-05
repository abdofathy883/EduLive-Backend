using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class LessonReportDTO
    {
        public string MemorizedContent { get; set; }
        public string ExplainedTopics { get; set; }
        public string DiscussedValues { get; set; }
        public string StudentId { get; set; }
        public string InstructorId { get; set; }
    }
}
