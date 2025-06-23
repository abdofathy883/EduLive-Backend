using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace eLearning_Admin.Pages.Admins
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;
        private readonly UserManager<BaseUser> userManager;

        public EditModel(Infrastructure.Data.E_LearningDbContext context, UserManager<BaseUser> user)
        {
            _context = context;
            userManager = user;
        }
        [BindProperty]
        public Core.Models.BaseUser Admin { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Users
                .Where(a => a is AdminUser)
                .Cast<AdminUser>()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admin is null)
            {
                return NotFound();
            }
            Admin = admin;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await userManager.UpdateAsync(Admin);
            }
            catch (Exception ex) // Fix: Catch System.Exception instead of IdentityError
            {
                if (!await UserExists(Admin.Id)) // Fix: Replace BlogExists with UserExists
                {
                    return NotFound();
                }
                else
                {
                    throw; // Rethrow the caught exception
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> UserExists(string id) // Fix: Add a method to check if the user exists
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
    }
}
