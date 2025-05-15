using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ZoomMeeting: IAuditable, IDeletable
    {
        public Guid Id { get; set; }
        public string ZoomMeetingId { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; } // in minutes
        public string JoinUrl { get; set; }
        public string StartUrl { get; set; }
        public string Password { get; set; }
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
