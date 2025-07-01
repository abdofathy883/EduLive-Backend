using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICourse courseService;
        public CategoryController(ICourse service)
        {
            courseService = service;
        }

        [HttpGet("all-categories")]
        public async Task<ActionResult<Category>> GetAllCoursesCategoriesAsync()
        {
            var categories = await courseService.GetAllCategoriesAsync();
            if (categories.Count == 0) return NotFound();
            return Ok(categories);
        }

        [HttpGet("get-category/{categoryId}")]
        public async Task<ActionResult<Category>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await courseService.GetCategoryByIdAsync(categoryId);
            if (category is null) return NotFound();
            return Ok(category);
        }
    }
}
