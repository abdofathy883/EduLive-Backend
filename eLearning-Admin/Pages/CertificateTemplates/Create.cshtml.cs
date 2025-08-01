﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.CertificateTemplates
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
            return Page();
        }

        [BindProperty]
        public CertificateTemplate CertificateTemplate { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CertificateTemplates.Add(CertificateTemplate);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
