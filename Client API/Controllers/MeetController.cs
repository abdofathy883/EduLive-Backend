using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetController : ControllerBase
    {
        private readonly IGoogleMeetService meetService;
        public MeetController(IGoogleMeetService googleMeetService)
        {
            meetService = googleMeetService;
        }

        [HttpPost("create-meet-leeson")]
        public async Task<IActionResult> CreateMeeting([FromBody] CreateGoogleMeetMeetingDTO request)
        {
            try
            {
                var meeting = await meetService.CreateMeetingAsync(request);
                return Ok(meeting);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("get-meeting-by-id/{lessonId}")]
        public async Task<IActionResult> GetMeetingById(int lessonId)
        {
            if (lessonId == 0)
            {
                return BadRequest();
            }
            try
            {
                var meeting = await meetService.GetMeetingByIdAsync(lessonId);
                return Ok(meeting);
            }
            catch (Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("get-meetings-by-course/{courseId}")]
        public async Task<IActionResult> GetMeetingsByCourseId(int courseId)
        {
            if (courseId == 0)
            {
                return BadRequest();
            }
            var meetings = await meetService.GetMeetingsByCourseIdAsync(courseId);
            return Ok(meetings);
        }

        [HttpGet("get-meetings-by-instructor/{instructorId}")]
        public async Task<IActionResult> GetMeetingsByInstructorId(string instructorId)
        {
            if (instructorId == null)
            {
                return BadRequest();
            }
            var meetings = await meetService.GetMeetingsByInstructorIdAsync(instructorId);
            return Ok(meetings);
        }

        [HttpGet("get-meetings-by-student/{studentId}")]
        public async Task<IActionResult> GetMeetingsByStudentId(string studentId)
        {
            if (studentId == null)
            {
                return BadRequest();
            }
            var meetings = await meetService.GetMeetingsByStudentIdAsync(studentId);
            return Ok(meetings);
        }
    }
}
