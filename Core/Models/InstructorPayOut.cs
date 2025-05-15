using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InstructorPayOut: IAuditable, IDeletable
    {
        public int Id { get; set; }
        public string InstructorUserId { get; set; }
        public virtual InstructorUser Instructor { get; set; }
        public string StripeTransferId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public PayOutStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
