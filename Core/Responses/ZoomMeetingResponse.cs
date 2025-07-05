using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Responses
{
    internal class ZoomMeetingResponse
    {
        public string Id { get; set; }
        public string JoinUrl { get; set; }
        public string start_url { get; set; }
        public string password { get; set; }
        public string Start_Time { get; set; }
        public int Duration { get; set; }
    }
}
