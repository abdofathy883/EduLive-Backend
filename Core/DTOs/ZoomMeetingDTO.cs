using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ZoomMeetingDTO
    {
        public Guid Id { get; set; }
        public string ZoomMeetingId { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string JoinUrl { get; set; }
        public string StartUrl { get; set; }
        public string Password { get; set; }
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
