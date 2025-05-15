using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CoursePaymentInfo
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public decimal Price { get; set; }
        public string StripePriceId { get; set; }
        public string StripeProductId { get; set; }
        public bool IsSubscriptionOnly { get; set; }
    }
}
