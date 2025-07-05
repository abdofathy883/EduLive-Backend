using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class GoogleMeetMeetingDTO
    {
        public string Topic { get; set; }
        public string GoogleEventId { get; set; }
        public string GoogleMeetURL { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }

        public string GoogleMeetUrl { get; set; }

        public int CourseId { get; set; }
        public int LessonId { get; set; }
        public string InstructorId { get; set; }
        public string StudentId { get; set; }
    }
}
