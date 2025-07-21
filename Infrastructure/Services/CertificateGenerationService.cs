using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using DinkToPdf;
using Infrastructure.Background;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CertificateGenerationService : ICertificateGenerationService
    {
        private readonly IGenericRepo<Course> courseRepo;
        private readonly IGenericRepo<CertificateTemplate> certificateTemplateRepo;
        private readonly IGenericRepo<CertificateIssued> certificateIssuedRepo;
        private readonly UserManager<BaseUser> userRepo;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CertificateGenerationService(IGenericRepo<Course> _courseRepo,
            UserManager<BaseUser> _userManager,
            IGenericRepo<CertificateTemplate> _certificateTemplateRepo,
            IGenericRepo<CertificateIssued> certificateIssuedRepo,
            IWebHostEnvironment webHost)
        {
            courseRepo = _courseRepo;
            userRepo = _userManager;
            certificateTemplateRepo = _certificateTemplateRepo;
            this.certificateIssuedRepo = certificateIssuedRepo;
            webHostEnvironment = webHost;
        }
        public async Task<string> GenerateCertificatePdfAsync(int templateId, string instructorId, string studentId, int courseId)
        {
            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(instructorId) || courseId == 0)
            {
                throw new ArgumentNullException("Insuffecient Data");
            }
            var course = await courseRepo.GetByIdAsync(courseId)
                ?? throw new KeyNotFoundException("Course not found.");

            var template = await certificateTemplateRepo.GetByIdAsync(templateId)
                ?? throw new KeyNotFoundException("Certificate template not found.");

            var student = await userRepo.FindByIdAsync(studentId)
                ?? throw new KeyNotFoundException("Student not found.");

            var instructor = await userRepo.FindByIdAsync(instructorId)
                ?? throw new KeyNotFoundException("Instructor not found.");

            
            if (!course.EnrolledStudents.Any(s => s.Id == studentId))
                throw new InvalidOperationException("Student not enrolled in this course.");

            var existingCert = await certificateIssuedRepo.FindAsync(c =>
            c.CourseId == courseId && c.StudentId == studentId && !c.IsDeleted);

            if (existingCert != null)
            {
                var firstExistingCert = existingCert.FirstOrDefault();
                return Path.Combine("/" + firstExistingCert.PdfPath.Replace("\\", "/"));
            }

            var StudentFullName = student.FirstName + " " + student.LastName;
            var InstructorFullName = instructor.FirstName + " " + instructor.LastName;

            var htmlFilePath = template.HTMLFilePath;

            // Read HTML content from file
            var html = await File.ReadAllTextAsync(htmlFilePath);

            // Replace placeholders
            html = html.Replace("{{StudentName}}", StudentFullName)
                       .Replace("{{CourseName}}", course.Title)
                       .Replace("{{InstructorName}}", InstructorFullName)
                       .Replace("{{IssueDate}}", DateTime.UtcNow.ToShortDateString());

            // Generate PDF from HTML (using your preferred library)
            var pdfBytes = GeneratePdfFromHtml(html); // implement this

            var serialNumber = new CertificateSerialNumberGenerator().Next(null); // Generate serial number
            var cert = new CertificateIssued
            {
                SerialNumber = serialNumber,
                IssueDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TemplateId = templateId,
                CourseId = courseId,
                StudentId = studentId,
                InstructorId = instructor.Id,
                CreatedAt = DateTime.UtcNow,
                PdfPath = Path.Combine("pdfs", $"{serialNumber}.pdf"),
            };

            await certificateIssuedRepo.AddAsync(cert);

            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "pdfs", $"{serialNumber}.pdf");
            await File.WriteAllBytesAsync(filePath, pdfBytes);

            return filePath;
        }

        public async Task<byte[]> GetCertificatePdfBytesAsync(string serialNumber)
        {
            var cert = await certificateIssuedRepo.GetByIdAsync(serialNumber)
                ?? throw new KeyNotFoundException("Certificate not found.");

            if (cert.IsDeleted)
                throw new KeyNotFoundException("Certificate is Deleted");

            var path = Path.Combine(webHostEnvironment.WebRootPath, cert.PdfPath);

            if (!File.Exists(path))
                throw new FileNotFoundException("PDF file not found.");

            return await File.ReadAllBytesAsync(path);
        }

        public async Task<List<CertificateDTO>> GetStudentCertificatesAsync(string studentId)
        {
            var certs = await certificateIssuedRepo
                .FindAsync(c => c.StudentId == studentId && !c.IsDeleted);

            // Convert the result to IQueryable to enable Include
            var certsQueryable = certs.AsQueryable();

            // Use Include to load related entities
            var certsWithIncludes = certsQueryable
                .Include(c => c.Course)
                .Include(c => c.Template);

            return certsWithIncludes.Select(c => new CertificateDTO
            {
                SerialNumber = c.SerialNumber,
                CourseName = c.Course.Title,
                TemplateTitle = c.Template.Title,
                IssueDate = c.IssueDate,
                PdfDownloadUrl = "/" + c.PdfPath.Replace("\\", "/")
            }).ToList();
        }

        private byte[] GeneratePdfFromHtml(string html)
        {
            var converter = new BasicConverter(new PdfTools());
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Landscape
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            return converter.Convert(doc);
        }
    }
}
