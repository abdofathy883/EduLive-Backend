using Core.Enums;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eLearning_Admin.Pages.Admins
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext context;
        private readonly UserManager<BaseUser> userManager;
        public CreateModel(E_LearningDbContext e_LearningDb, UserManager<BaseUser> user)
        {
            context = e_LearningDb;
            userManager = user;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public BaseUser Admin { get; set; } = default!;
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Admin?.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return Page();
            }

            // Create a new AdminUser object
            var admin = new AdminUser
            {
                AdminId = Guid.NewGuid().ToString(),
                FirstName = Admin.FirstName,
                LastName = Admin.LastName,
                Email = Admin.Email,
                UserName = Admin.Email.Split("@")[0], // Safe to use as Admin.Email is checked for null or empty
                PhoneNumber = Admin.PhoneNumber,
                DateOfBirth = Admin.DateOfBirth,
                PasswordHash = userManager.PasswordHasher.HashPassword(Admin, Admin.PasswordHash),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                Notifications = new List<Notification>(),
                RefreshTokens = new List<RefreshToken>()
            };

            // Add the new admin to the context
            var result = await userManager.CreateAsync(admin, Admin.PasswordHash);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, UserRoles.Admin.ToString());
                return RedirectToPage("./Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
