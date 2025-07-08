using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourse courseService;
        private readonly IAuth auth;
        public CoursesController(ICourse service, IAuth _auth)
        {
            courseService = service;
            auth = _auth;
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
        public async Task<ActionResult<List<Course>>> GetAllCoursesAsync()
        {
            var courseList = await courseService.GetAllCoursesAsync();
            if (courseList.Count == 0) return NotFound();
            return Ok(courseList);
        }
        [HttpGet("enrolled-course/{studentId}")]
        public async Task<ActionResult<CourseDTO>> GetEnrolledCoursesAsync(string studentId)
        {
            var enrolledCourses = await courseService.GetEnrolledCoursesAsync(studentId);
            if (enrolledCourses is null) return NotFound();
            return Ok(enrolledCourses);
        }
        [HttpGet("owned-course/{instructorId}")]
        public async Task<ActionResult<List<CourseDTO>>> GetOwnedCoursesAsync(string instructorId)
        {
            var ownedCourses = await courseService.GetOwnedCoursesAsync(instructorId);
            if (ownedCourses is null) return NotFound();
            return Ok(ownedCourses);
        }

        [HttpGet("get-instructor/{instructorId}")]
        public async Task<ActionResult<InstructorUser>> GetInstructorByIdAsync(string instructorId)
        {
            var instructor = await auth.GetInstructorByIdAsync(instructorId);
            if (instructor is null) return NotFound();
            return Ok(instructor);
        }
        [HttpGet("get-course/{courseId}")]
        public async Task<ActionResult<Course>> GetCourseByIdAsync(int courseId)
        {
            var course = await courseService.GetCourseByIdAsync(courseId);
            if (course is null) return NotFound();
            return Ok(course);
        }
        [HttpGet("get-students-by-course/{courseId}")]
        public async Task<ActionResult<List<AuthDTO>>> GetStudentsByCourseIdAsync(int courseId)
        {
            var course = await courseService.GetCourseByIdAsync(courseId);
            if (course is null) return NotFound();

            var students = course.EnrolledStudents.Select(s => new AuthDTO
            {
                UserId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
            }).ToList();
            return Ok(students);
        }
    }
}
