using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.LessonReports
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public CreateModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["InstructorId"] = new SelectList(_context.Set<InstructorUser>(), "Id", "Id");
        ViewData["StudentId"] = new SelectList(_context.Set<StudentUser>(), "Id", "Id");
            return Page();
        }

        [BindProperty]
        public LessonReport LessonReport { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.LessonReports.Add(LessonReport);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
