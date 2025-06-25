using Core.Interfaces;
using Microsoft.AspNetCore.Http;
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
        public required decimal OriginalPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public required string CourseImagePath { get; set; }
        [NotMapped]
        public IFormFile CourseImage { get; set; }
        [ForeignKey("Category")]
        public virtual int CategoryId { get; set; }
        public Category Category { get; set; }
        [ForeignKey("InstructorUser")]
        public virtual string InstructorId { get; set; }
        public InstructorUser Instructor { get; set; }
        public string? CertificateTemplatePath { get; set; }
        public List<Certificate> IssuedCertificates { get; set; } = new List<Certificate>();
        public List<StudentUser> EnrolledStudents { get; set; } = new();
        public List<Lesson> Lessons { get; set; } = new();
        public List<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
        public List<Payment> Purchases { get; set; } = new List<Payment>();
        public bool IsDeleted { get ; set ; }
        public DateTime CreatedAt { get ; set ; }
        public DateTime? UpdatedAt { get ; set; }
    }
}
