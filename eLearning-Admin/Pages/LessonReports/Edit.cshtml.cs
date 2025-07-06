using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.LessonReports
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public EditModel(Infrastructure.Data.E_LearningDbContext context)
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

            var lessonreport =  await _context.LessonReports.FirstOrDefaultAsync(m => m.Id == id);
            if (lessonreport == null)
            {
                return NotFound();
            }
            LessonReport = lessonreport;
           ViewData["InstructorId"] = new SelectList(_context.Set<InstructorUser>(), "Id", "Id");
           ViewData["StudentId"] = new SelectList(_context.Set<StudentUser>(), "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LessonReport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonReportExists(LessonReport.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LessonReportExists(int id)
        {
            return _context.LessonReports.Any(e => e.Id == id);
        }
    }
}
