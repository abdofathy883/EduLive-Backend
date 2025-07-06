using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.GoogleMeetLessons
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public DeleteModel(Infrastructure.Data.E_LearningDbContext context)
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

            var googlemeetlesson = await _context.GoogleMeetLessons.FirstOrDefaultAsync(m => m.Id == id);

            if (googlemeetlesson is not null)
            {
                GoogleMeetLesson = googlemeetlesson;

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

            var googlemeetlesson = await _context.GoogleMeetLessons.FindAsync(id);
            if (googlemeetlesson != null)
            {
                GoogleMeetLesson = googlemeetlesson;
                _context.GoogleMeetLessons.Remove(GoogleMeetLesson);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
