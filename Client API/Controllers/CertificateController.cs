using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICertificateGenerationService CertificateGenerationService;
        public CertificateController(IHttpContextAccessor httpContextAccessor, ICertificateGenerationService service)
        {
            _httpContextAccessor = httpContextAccessor;
            CertificateGenerationService = service;
        }

        [HttpGet("get-student-certificates/{studentId}")]
        public async Task<IActionResult> GetStudentCertificates(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest("Student ID is required");
            }
            var studentCertificates = await CertificateGenerationService.GetStudentCertificatesAsync(studentId);
            if (studentCertificates == null || !studentCertificates.Any())
            {
                return NotFound("No certificates found for this student.");
            }
            return Ok(studentCertificates);
        }

        [HttpGet("download/{serialNumber}")]
        public async Task<IActionResult> DownloadCertificate(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return BadRequest("Serial number is required");
            }
            try
            {
                var certificate = await CertificateGenerationService.GetCertificatePdfBytesAsync(serialNumber);
                return File(certificate, "application/pdf", $"{serialNumber}.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("generate-certificate")]
        public async Task<IActionResult> IssueNewCertificate([FromBody] IssueCertificateRequest certificateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var pdfURL = await CertificateGenerationService.GenerateCertificatePdfAsync(
                    certificateRequest.TemplateId,
                    certificateRequest.StudentId,
                    certificateRequest.InstructorId,
                    certificateRequest.CourseId);
                return Ok(new
                {
                    Message = "Certificate generated successfully.",
                    pdfURL
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
