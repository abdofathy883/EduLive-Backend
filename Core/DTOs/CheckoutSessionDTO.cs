using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CheckoutSessionDTO
    {
        public int Id { get; set; }
        public string PaymentStatus { get; set; }
        public string CustomerEmail { get; set; }
        public double AmountTotla { get; set; }
    }
}
