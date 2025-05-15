using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CourseDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int NuOfLessons { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal? SalePrice { get; set; }
        public IFormFile CourseImage { get; set; }

        public int CategoryId { get; set; }

        public int InstructorId { get; set; }

        public string CertificateSerialNumber { get; set; }
    }
}
