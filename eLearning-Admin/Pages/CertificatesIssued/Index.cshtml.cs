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
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public IndexModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        public IList<CertificateIssued> CertificateIssued { get;set; } = default!;

        public async Task OnGetAsync()
        {
            CertificateIssued = await _context.CertificateIssueds
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Student)
                .Include(c => c.Template).ToListAsync();
        }
    }
}
