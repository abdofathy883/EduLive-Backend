using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Payment: IAuditable, IDeletable
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public virtual StudentUser Student { get; set; }
        public int? CourseId { get; set; }
        public virtual Course Course { get; set; }
        public int? SubscriptionId { get; set; }
        public virtual Subscription Subscription { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string StripeChargeId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
