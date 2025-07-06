using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.ZoomLessons
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public DeleteModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ZoomMeeting ZoomMeeting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zoommeeting = await _context.ZoomMeetings.FirstOrDefaultAsync(m => m.Id == id);

            if (zoommeeting is not null)
            {
                ZoomMeeting = zoommeeting;

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

            var zoommeeting = await _context.ZoomMeetings.FindAsync(id);
            if (zoommeeting != null)
            {
                ZoomMeeting = zoommeeting;
                _context.ZoomMeetings.Remove(ZoomMeeting);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
