using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CheckoutSessionDTO
    {
        public string StudentId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
    }
}
