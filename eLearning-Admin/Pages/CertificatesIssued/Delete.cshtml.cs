using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.CertificatesIssued
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public DeleteModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CertificateIssued CertificateIssued { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificateissued = await _context.CertificateIssueds.FirstOrDefaultAsync(m => m.SerialNumber == id);

            if (certificateissued is not null)
            {
                CertificateIssued = certificateissued;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificateissued = await _context.CertificateIssueds.FindAsync(id);
            if (certificateissued != null)
            {
                CertificateIssued = certificateissued;
                _context.CertificateIssueds.Remove(CertificateIssued);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
