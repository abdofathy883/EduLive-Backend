using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourse courseService;
        public CoursesController(ICourse service)
        {
            courseService = service;
        }
        [HttpPost("add-new-course")]
        public async Task<ActionResult<CourseDTO>> AddCourseAsync([FromForm] CourseDTO course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newCourse = await courseService.AddCourseAsync(course);
            if (newCourse is null)
            {
                return NotFound();
            }
            return base.Ok((object)newCourse);
        }
        [HttpPut("update-course")]
        public async Task<ActionResult<CourseDTO>> UpdateCourseAsync(int oldCourseId, [FromForm] CourseDTO newCourse)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var oldCourse = await courseService.GetCourseByIdAsync(oldCourseId);
            if (oldCourse is null) return NotFound();
            var result = await courseService.UpdateCourseAsync(oldCourseId, newCourse);
            return Ok(result);
        }

        [HttpDelete("delete-course")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var course = await courseService.GetCourseByIdAsync(courseId);
            if (course is null) return NotFound();
            var result = await courseService.DeleteCourseAsync(courseId);
            return Ok(result);
        }
        [HttpGet("all-courses")]
        public async Task<ActionResult<List<CourseDTO>>> GetAllCoursesAsync()
        {
            var courseList = await courseService.GetAllCoursesAsync();
            if (courseList.Count == 0) return NotFound();
            return Ok(courseList);
        }
        [HttpGet("enrolled-course")]
        public async Task<ActionResult<CourseDTO>> GetEnrolledCoursesAsync(int studentId)
        {
            var enrolledCourses = await courseService.GetEnrolledCoursesAsync(studentId);
            if (enrolledCourses is null) return NotFound();
            return Ok(enrolledCourses);
        }
        [HttpGet("owned-course")]
        public async Task<ActionResult<CourseDTO>> GetOwnedCoursesAsync(int instructorId)
        {
            var ownedCourses = await courseService.GetOwnedCoursesAsync(instructorId);
            if (ownedCourses is null) return NotFound();
            return Ok(ownedCourses);
        }
    }
}
