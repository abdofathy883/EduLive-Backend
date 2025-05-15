using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CreateZoomMeetingDTO
    {
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
