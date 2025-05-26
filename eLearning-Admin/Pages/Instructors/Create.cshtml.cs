using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eLearning_Admin.Pages.Instructors
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext context;
        private readonly IAuth authService;
        public CreateModel(E_LearningDbContext e_LearningDb, IAuth auth)
        {
            context = e_LearningDb;
            authService = auth;
        }
        public IActionResult OnGet()
        {

            return Page();
        }
        [BindProperty]
        public InstructorRegisterDTO Instructor { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Create a new InstructorUser object
            var instructor = new InstructorRegisterDTO
            {
                FirstName = Instructor.FirstName,
                LastName = Instructor.LastName,
                Email = Instructor.Email,
                PhoneNumber = Instructor.PhoneNumber,
                DateOfBirth = Instructor.DateOfBirth,
                CvPath = Instructor.CvPath,
                VideoPath = Instructor.VideoPath,
                Bio = Instructor.Bio,
                Password = Instructor.Password,
                ConfirmPassword = Instructor.Password,
            };
            // Add the new instructor to the context
            await authService.InstructorRegisterAsync(instructor);
            return RedirectToPage("./Index");
        }
    }
}
