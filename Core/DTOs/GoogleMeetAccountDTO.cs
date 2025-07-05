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
        public required string UserId { get; set; }
        public required string GoogleUserId { get; set; }
        public required string Email { get; set; }
        public bool IsConnected { get; set; }
    }
}
