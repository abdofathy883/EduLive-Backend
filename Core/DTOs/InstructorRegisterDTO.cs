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
        public string CvPath { get; set; }
        public string VideoPath { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
