using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class InstructorRegisterDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public IFormFile? CvPath { get; set; }
        public IFormFile? VideoPath { get; set; }
        public string? Bio { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
