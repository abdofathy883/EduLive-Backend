using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int UserId { get; set; }
        public DateTime SchudledTime { get; set; }
        public DateTime? ActualSentTime { get; set; }
        public NotificationStatus Status { get; set; }
        public string MessageContent { get; set; }
    }
}
