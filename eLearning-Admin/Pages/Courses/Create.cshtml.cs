using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.Models;
using Infrastructure.Data;
using Core.Interfaces;
using Core.DTOs;

namespace eLearning_Admin.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.E_LearningDbContext _context;
        private readonly ICourse courseService;

        public CreateModel(Infrastructure.Data.E_LearningDbContext context, ICourse course)
        {
            _context = context;
            courseService = course;
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title");
        ViewData["InstructorId"] = new SelectList(_context.InstructorUsers, "Id", "FirstName");
            return Page();
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Save the uploaded file to a specific location and get the file path
            //string filePath = Path.Combine("wwwroot/uploads", Course.CourseImage.FileName);
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await Course.CourseImage.CopyToAsync(stream);
            //}

            // Map the Course model to a CourseDTO object
            var courseDto = new CourseDTO
            {
                Title = Course.Title,
                Description = Course.Description,
                NuOfLessons = Course.NuOfLessons,
                OriginalPrice = Course.OriginalPrice,
                SalePrice = Course.SalePrice,
                CourseImage = Course.CourseImage, // Use the saved file path
                CategoryId = Course.CategoryId,
                InstructorId = Course.InstructorId,
                CertificateSerialNumber = Course.CertificateTemplatePath
            };

            await courseService.AddCourseAsync(courseDto);

            return RedirectToPage("./Index");
        }
    }
}
