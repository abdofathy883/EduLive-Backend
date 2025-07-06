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

namespace eLearning_Admin.Pages.Transactions
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public EditModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Payment Payment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment =  await _context.Payments.FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }
            Payment = payment;
           ViewData["CourseId"] = new SelectList(_context.Courses, "ID", "CourseImagePath");
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

            _context.Attach(Payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(Payment.Id))
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

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
