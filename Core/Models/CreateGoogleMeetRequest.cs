using Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CreateGoogleMeetRequest
    {
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public GoogleMeetSettings Settings { get; set; }
    }
}
