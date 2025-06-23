using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InstructorUser : BaseUser
    {
        [Required]
        public string InstructorId { get; set; }
        public List<Lesson>? Lessons {  get; set; }
        public List<Course>? Courses {  get; set; }
        public List<Certificate>? IssuedCertificates { get; set; }
        public string? IntroVideoPath { get; set; }
        [NotMapped]
        public IFormFile? IntroVideo { get; set; }
        public string? CVPath { get; set; }
        [NotMapped]
        public IFormFile? CV { get; set; }
        public string? StripeConnectAccountId { get; set; }
        public bool HasCompletedStripeOnboarding { get; set; }
        public List<InstructorReview>? InstructorReviews { get; set; }
        public virtual List<InstructorPayOut> Payouts { get; set; } = new List<InstructorPayOut>();
        public string? Bio { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}
