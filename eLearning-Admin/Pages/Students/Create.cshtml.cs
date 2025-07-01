using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eLearning_Admin.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly IAuth authService;
        public CreateModel(IAuth auth)
        {
            authService = auth;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public RegisterDTO Student { get; set; } = default!;
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var student = new RegisterDTO
            {
                FirstName = Student.FirstName,
                LastName = Student.LastName,
                Email = Student.Email,
                PhoneNumber = Student.PhoneNumber,
                DateOfBirth = Student.DateOfBirth,
                Password = Student.Password,
            };

            await authService.RegisterAsync(student);
            return RedirectToPage("./Index");
        }
    }
}
