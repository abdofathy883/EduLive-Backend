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

namespace eLearning_Admin.Pages.GoogleMeetLessons
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public EditModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GoogleMeetLesson GoogleMeetLesson { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var googlemeetlesson =  await _context.GoogleMeetLessons.FirstOrDefaultAsync(m => m.Id == id);
            if (googlemeetlesson == null)
            {
                return NotFound();
            }
            GoogleMeetLesson = googlemeetlesson;
           ViewData["LessonId"] = new SelectList(_context.Lessons, "LessonId", "InstructorId");
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

            _context.Attach(GoogleMeetLesson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoogleMeetLessonExists(GoogleMeetLesson.Id))
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

        private bool GoogleMeetLessonExists(int id)
        {
            return _context.GoogleMeetLessons.Any(e => e.Id == id);
        }
    }
}
