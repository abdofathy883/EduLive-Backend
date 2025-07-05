using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonReportController : ControllerBase
    {
        private readonly ILessonReport lessonReportService;
        public LessonReportController(ILessonReport _lessonReportService)
        {
            lessonReportService = _lessonReportService;
        }
        [HttpPost("AddReport")]
        public async Task<IActionResult> AddReport(LessonReportDTO newLesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await lessonReportService.AddReportAsync(newLesson);
            return Ok(result);
        }
        [HttpGet("InstructorReports/{instructorId}")]
        public async Task<IActionResult> GetReportsByInstructorId(string instructorId)
        {
            var reports = await lessonReportService.GetReportsByInstructorIdAsync(instructorId);
            return Ok(reports);
        }
        [HttpGet("StudentReports/{studentId}")]
        public async Task<IActionResult> GetReportsByStudentId(string studentId)
        {
            var reports = await lessonReportService.GetReportsByStudentIdAsync(studentId);
            return Ok(reports);
        }
    }
}
