using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;

namespace eLearning_Admin.Pages.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;

        public IndexModel(Infrastructure.Data.E_LearningDbContext context)
        {
            _context = context;
        }

        public IList<Payment> Payment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Payment = await _context.Payments
                .Include(p => p.Course)
                .Include(p => p.Student).ToListAsync();
        }
    }
}
