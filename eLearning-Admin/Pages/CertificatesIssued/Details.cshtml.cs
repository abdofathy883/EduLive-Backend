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
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public DetailsModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

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
    }
}
