using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorReviewsController : ControllerBase
    {
        private readonly IReviewsService reviews;
        public InstructorReviewsController(IReviewsService service)
        {
            reviews = service;
        }

        [HttpGet("get-instructor-reviews")]
        public async Task<IActionResult> GetAllInstructorReviesAsync()
        {
            var reviewList = await reviews.GetAllReviewsAsync();
            if (reviewList == null) return NotFound();
            return Ok(reviewList);
        }
    }
}
