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
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public IndexModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        public IList<CertificateTemplate> CertificateTemplate { get;set; } = default!;

        public async Task OnGetAsync()
        {
            CertificateTemplate = await _context.CertificateTemplates.ToListAsync();
        }
    }
}
