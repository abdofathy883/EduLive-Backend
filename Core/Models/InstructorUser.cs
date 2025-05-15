using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InstructorUser : BaseUser
    {
        public int InstructorId { get; set; }
        public List<Lesson>? Lessons {  get; set; }
        public List<Course>? Courses {  get; set; }
        public List<StudentUser> Students { get; set; }
        //public List<Notification> Notifications { get; set; }
        public string? IntroVideoPath { get; set; }
        public IFormFile? IntroVideo { get; set; }
        public string? CVPath { get; set; }
        public IFormFile? CV { get; set; }
        public string StripeConnectAccountId { get; set; }
        public bool HasCompletedStripeOnboarding { get; set; }
        public virtual List<InstructorPayOut> Payouts { get; set; } = new List<InstructorPayOut>();
        public string Bio { get; set; }
    }
}
