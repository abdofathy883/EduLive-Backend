using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace eLearning_Admin.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly E_LearningDbContext context;
        public IndexModel(E_LearningDbContext dbContext)
        {
            context = dbContext;
        }
        public IList<StudentUser> Students { get; set; } = default!;
        public async Task OnGet()
        {
            if (context.Users != null)
            {
                Students = await context.Users
                    .OfType<StudentUser>()
                    .Where(u => u.IsDeleted == false)
                    .ToListAsync();
            }
            else
            {
                Students = new List<StudentUser>();
            }
        }

        public async Task<IActionResult> OnPostAsync(int? deleteEntryId)
        {
            if (deleteEntryId == null)
            {
                return NotFound();
            }

            var student = await context.Users.FindAsync(deleteEntryId);
            if (student != null)
            {
                context.Users.Remove(student);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
