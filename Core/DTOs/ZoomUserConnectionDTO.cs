using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ZoomUserConnectionDTO
    {
        public required string UserId { get; set; }
        public required string ZoomUserId { get; set; }
        public required string ZoomEmail { get; set; }
        public bool IsConnected { get; set; }
    }
}
