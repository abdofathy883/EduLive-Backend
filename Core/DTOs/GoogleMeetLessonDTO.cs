using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class GoogleMeetLessonDTO
    {
        public int Id { get; set; }
        public string GoogleMeetId { get; set; }
        public string JoinUrl { get; set; }
        public string MeetingCode { get; set; }
        public int LessonId { get; set; }
    }
}
