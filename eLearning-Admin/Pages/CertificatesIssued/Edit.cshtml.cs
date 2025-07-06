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

namespace eLearning_Admin.Pages.CertificatesIssued
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public EditModel(Infrastructure.Data.E_LearningDbContext context)
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

            var certificateissued =  await _context.CertificateIssueds.FirstOrDefaultAsync(m => m.SerialNumber == id);
            if (certificateissued == null)
            {
                return NotFound();
            }
            CertificateIssued = certificateissued;
           ViewData["CourseId"] = new SelectList(_context.Courses, "ID", "CourseImagePath");
           ViewData["InstructorId"] = new SelectList(_context.Set<InstructorUser>(), "Id", "Id");
           ViewData["StudentId"] = new SelectList(_context.Set<StudentUser>(), "Id", "Id");
           ViewData["TemplateId"] = new SelectList(_context.CertificateTemplates, "Id", "HTMLFilePath");
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

            _context.Attach(CertificateIssued).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CertificateIssuedExists(CertificateIssued.SerialNumber))
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

        private bool CertificateIssuedExists(string id)
        {
            return _context.CertificateIssueds.Any(e => e.SerialNumber == id);
        }
    }
}
