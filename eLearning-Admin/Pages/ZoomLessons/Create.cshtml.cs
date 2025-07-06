using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.ZoomLessons
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
        ViewData["LessonId"] = new SelectList(_context.Lessons, "LessonId", "InstructorId");
            return Page();
        }

        [BindProperty]
        public ZoomMeeting ZoomMeeting { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ZoomMeetings.Add(ZoomMeeting);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
