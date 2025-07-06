using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CertificateIssued : IAuditable, IDeletable
    {
        [Key]
        public required string SerialNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string PdfPath { get; set; }
        public int TemplateId { get; set; }
        public CertificateTemplate Template { get; set; }
        [ForeignKey("Course")]
        public virtual int CourseId { get; set; }
        public Course Course { get; set; }
        [ForeignKey("StudentUser")]
        public virtual string StudentId { get; set; }
        public StudentUser Student { get; set; }
        [ForeignKey("InstructorUser")]
        public virtual string InstructorId { get; set; }
        public InstructorUser Instructor { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
