using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Subscription: IAuditable, IDeletable
    {
        public int Id { get; set; }
        public string StudentUserId { get; set; }
        public virtual StudentUser Student { get; set; }
        public int PaymentPlanId { get; set; }
        public virtual PaymentPlan PaymentPlan { get; set; }
        public string StripeSubscriptionId { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime? CurrentPeriodStart { get; set; }
        public DateTime? CurrentPeriodEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
