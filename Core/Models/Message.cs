using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Message : IDeletable
    {
        public int Id { get; set; }
        public string SenderId { get; set; }  // BaseUser Id
        public string ReceiverId { get; set; } // BaseUser Id
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public BaseUser Sender { get; set; }
        public BaseUser Receiver { get; set; }
        public bool IsDeleted { get; set; }
    }
}
