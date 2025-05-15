using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Responses
{
    public class GoogleMeetAuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string Email { get; set; }
        public string GoogleUserId { get; set; }
    }
}
