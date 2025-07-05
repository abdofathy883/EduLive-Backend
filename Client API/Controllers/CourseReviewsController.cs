using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseReviewsController : ControllerBase
    {
        private readonly ICourseReviews courseReview;
        public CourseReviewsController(ICourseReviews _courseReview)
        {
            courseReview = _courseReview;
        }
        [HttpPost("add-review")]
        public async Task<IActionResult> AddReviewAsync([FromBody] CourseReviewDTO review)
        {
            if (review == null || string.IsNullOrEmpty(review.Comment) || review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest("Invalid review data.");
            }
            try
            {
                await courseReview.AddReviewAsync(review);
                return Ok(new { Message = "Review added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding review: {ex.Message}");
            }
        }

        [HttpGet("get-reviews-by-course/{courseId}")]
        public async Task<ActionResult<CourseReview>> GetReviewByCourseId(int courseId)
        {
            var courseReviews = await courseReview.GetReviewsByCourseIdAsync(courseId);
            if (courseReviews is null)
            {
                return NotFound();
            }
            return Ok(courseReviews);
        }

        [HttpGet("get-reviews-by-student/{studentId}")]
        public async Task<ActionResult<CourseReview>> GetReviewsByStudentId(string studentId)
        {
            var studentReviews = await courseReview.GetReviewsByStudentIdAsync(studentId);
            if (studentReviews is null)
            {
                return NotFound();
            }
            return Ok(studentReviews);
        }

        [HttpGet("get-average-rating-by-course/{courseId}")]
        public async Task<ActionResult<double>> GetAverageRatingByCourseId(int courseId)
        {
            var averageRating = await courseReview.GetAverageRatingByCourseIdAsync(courseId);
            if (averageRating == null)
            {
                return NotFound();
            }
            return Ok(averageRating);
        }
    }
}
