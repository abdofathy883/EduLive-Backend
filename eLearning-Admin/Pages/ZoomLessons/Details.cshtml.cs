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
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public DetailsModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

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
    }
}
