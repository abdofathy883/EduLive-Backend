using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.LessonReports
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public DeleteModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LessonReport LessonReport { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessonreport = await _context.LessonReports.FirstOrDefaultAsync(m => m.Id == id);

            if (lessonreport is not null)
            {
                LessonReport = lessonreport;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessonreport = await _context.LessonReports.FindAsync(id);
            if (lessonreport != null)
            {
                LessonReport = lessonreport;
                _context.LessonReports.Remove(LessonReport);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
