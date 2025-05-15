using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService blogService;
        public BlogController(IBlogService blog)
        {
            blogService = blog;
        }

        [HttpGet("get-all-blogs")]
        public async Task<IActionResult> GetAllBlogsAsync()
        {
            var blogList = await blogService.GetAllBlogsAsync();
            if (blogList == null) return NotFound();
            return Ok(blogList);
        }
        [HttpGet("get-blog")]
        public async Task<IActionResult> GetBlogByIdAsync(int blogId)
        {
            var blog = await blogService.GetBlogByIDAsync(blogId);
            if (blog == null) return NotFound();
            return Ok(blog);
        }
    }
}
