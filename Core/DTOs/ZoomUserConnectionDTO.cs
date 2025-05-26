using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ZoomUserConnectionDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ZoomUserId { get; set; }
        public string ZoomEmail { get; set; }
        public bool IsConnected { get; set; }
    }
}
