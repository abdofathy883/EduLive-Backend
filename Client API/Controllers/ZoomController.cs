using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ZoomController : ControllerBase
    {
        private readonly IZoomService zoomService;

        public ZoomController(IZoomService zoom)
        {
            zoomService = zoom;
        }
        [HttpPost("add-new-zoom-lesson")]
        public async Task<ActionResult<ZoomMeetingDTO>> CreateZoomMeetingAsync(CreateZoomMeetingDTO zoomMeetingDTO)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await zoomService.CreateMeetingsAsync(zoomMeetingDTO);
            return Ok(result);
        }
        [HttpGet("get-zoom-meeting/{meetingId}")]
        public async Task<ActionResult<ZoomMeetingDTO>> GetZoomMeetingById(int meetingId)
        {
            var meeting = await zoomService.GetMeetingByIdAsync(meetingId);
            if (meeting is null)
            {
                return NotFound();
            }
            return Ok(meeting);
        }
        [HttpPut("update-zoom-meeting{meetingId}")]
        public async Task<IActionResult> UpdateZoomMeetingAsync(UpdateZoomMeetingDTO updateZoomMeeting)
        {
            if (updateZoomMeeting is null)
            {
                return NotFound();
            }
            await zoomService.UpdateMeetingAsync(updateZoomMeeting);
            return Ok();
        }

        [HttpGet("get-meetings-by-course/{courseId}")]
        public async Task<ActionResult<List<ZoomMeetingDTO>>> GetMeetingsByCourseIdAsync(int courseId)
        {
            var meetings = await zoomService.GetMeetingsByCourseIdAsync(courseId);
            if (meetings is null || meetings.Count == 0)
            {
                return NotFound();
            }
            return Ok(meetings);
        }
        [HttpGet("get-meetings-by-instructor/{instructorId}")]
        public async Task<ActionResult<List<ZoomMeetingDTO>>> GetMeetingsByInstructorIdAsync(string instructorId)
        {
            var meetings = await zoomService.GetMeetingsByInstructorIdAsync(instructorId);
            if (meetings is null || meetings.Count == 0)
            {
                return NotFound();
            }
            return Ok(meetings);
        }
        [HttpGet("get-meetings-by-student/{studentId}")]
        public async Task<ActionResult<List<ZoomMeetingDTO>>> GetMeetingsByStudentIdAsync(string studentId)
        {
            var meetings = await zoomService.GetMeetingsByStudentIdAsync(studentId);
            if (meetings is null || meetings.Count == 0)
            {
                return NotFound();
            }
            return Ok(meetings);
        }
    }
}
