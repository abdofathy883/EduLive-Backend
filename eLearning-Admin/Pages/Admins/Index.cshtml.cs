using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace eLearning_Admin.Pages.Admins
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext context;
        public IndexModel(E_LearningDbContext e_LearningDb)
        {
            context = e_LearningDb;
        }
        public IList<Core.Models.AdminUser> Admins { get; set; } = default!;
        public async Task OnGet()
        {
            if (context.Users != null)
            {
                Admins = await context.Users
                    .OfType<Core.Models.AdminUser>()
                    .Where(u => u.IsDeleted == false)
                    .ToListAsync();
            }
            else
            {
                Admins = new List<Core.Models.AdminUser>();
            }
        }
    }
}
