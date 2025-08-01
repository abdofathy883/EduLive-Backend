﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Models;
using Infrastructure.Data;
using Core.Interfaces;

namespace eLearning_Admin.Pages.Category
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;
        private readonly IGenericRepo<Core.Models.Category> repo;

        public CreateModel(Infrastructure.Data.E_LearningDbContext context, IGenericRepo<Core.Models.Category> genericRepo)
        {
            _context = context;
            repo = genericRepo;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Core.Models.Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await repo.AddAsync(Category);
            await repo.SaveAllAsync();
            //_context.Categories.Add(Category);
            //await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
