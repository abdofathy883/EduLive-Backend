using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class IssueCertificateRequest
    {
        public required int TemplateId { get; set; }
        public required int CourseId { get; set; }
        public required string StudentId { get; set; }
        public required string InstructorId { get; set; }
    }
}
