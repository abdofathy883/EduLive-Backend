using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class GoogleMeetLesson: IDeletable
    {
        public int Id { get; set; }
        public string GoogleMeetId { get; set; }
        public string JoinUrl { get; set; }
        public string MeetingCode { get; set; }

        // Foreign key relationship
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public bool IsDeleted { get; set; }
    }
}
