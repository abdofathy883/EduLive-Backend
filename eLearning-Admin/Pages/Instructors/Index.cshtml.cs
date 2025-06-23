using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace eLearning_Admin.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;
        public IndexModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }
        public IList<Core.Models.InstructorUser> Instructors { get; set; } = default!;
        public async Task OnGet()
        {
            if (_context.Users != null)
            {
                Instructors = await _context.Users
                    .OfType<Core.Models.InstructorUser>()
                    .Where(u => u.IsDeleted == false)
                    .ToListAsync();
            }
            else
            {
                Instructors = new List<Core.Models.InstructorUser>();
            }
        }

        public async Task<IActionResult> OnPostAsync(int? deleteEntryId)
        {
            if (deleteEntryId == null)
            {
                return NotFound();
            }

            var instructor = await _context.Users.FindAsync(deleteEntryId);
            if (instructor != null)
            {
                _context.Users.Remove(instructor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
