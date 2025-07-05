using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class GoogleMeetLesson: IAuditable, IDeletable
    {
        public int Id { get; set; }
        public string? GoogleEventId { get; set; }          // ID of the calendar event
        public string? GoogleMeetURL { get; set; }         // The Meet link (hangoutLink or entryPoint.uri)
        public string? GoogleCalendarId { get; set; }       // Usually "primary" or the specific calendar used
        public DateTime? StartTime { get; set; }            // Start time of the event
        public int Duration { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
       public DateTime? UpdatedAt { get; set; }
    }}
