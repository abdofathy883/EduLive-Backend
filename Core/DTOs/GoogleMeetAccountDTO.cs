using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class GoogleMeetAccountDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string GoogleUserId { get; set; }
        public bool IsConnected { get; set; }
    }
}
