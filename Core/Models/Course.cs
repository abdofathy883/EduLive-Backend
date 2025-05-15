using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Course : IDeletable, IAuditable
    {
        public int ID { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int NuOfLessons { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string CourseImagePath { get; set; }
        //public IFormFile
        [ForeignKey("Category")]
        public virtual int CategoryId { get; set; }
        [ForeignKey("InstructorUser")]
        public virtual int InstructorId { get; set; }
        [ForeignKey("Certificate")]
        public virtual string CertificateSerialNumber { get; set; }
        public List<StudentUser>? EnrolledStudents { get; set; }
        public List<Lesson>? lessons { get; set; }
        public bool IsDeleted { get ; set ; }
        public DateTime CreatedAt { get ; set ; }
        public DateTime? UpdatedAt { get ; set; }
    }
}
