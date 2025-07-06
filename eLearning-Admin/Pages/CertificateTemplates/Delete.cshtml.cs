using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.CertificateTemplates
{
    public class DeleteModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public DeleteModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CertificateTemplate CertificateTemplate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificatetemplate = await _context.CertificateTemplates.FirstOrDefaultAsync(m => m.Id == id);

            if (certificatetemplate is not null)
            {
                CertificateTemplate = certificatetemplate;

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

            var certificatetemplate = await _context.CertificateTemplates.FindAsync(id);
            if (certificatetemplate != null)
            {
                CertificateTemplate = certificatetemplate;
                _context.CertificateTemplates.Remove(CertificateTemplate);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
