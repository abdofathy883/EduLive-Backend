using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Infrastructure.Services
{
    public class MediaUploadsService
    {
        private readonly IHttpContextAccessor contextAccessor;
        public MediaUploadsService(IHttpContextAccessor httpContext)
        {
            contextAccessor = httpContext;
        }
        public async Task<string> UploadImage(IFormFile image, string courseName)
        {
            var originalExtension = Path.GetExtension(image.FileName).ToLower();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var sanitizedCourseName = string.Join("_", courseName.Split(Path.GetInvalidFileNameChars()));
            var fileNameWithoutExt = $"{sanitizedCourseName}_Tahfez-Quran";
            var webpFileName = fileNameWithoutExt + ".webp";
            var webpFilePath = Path.Combine(uploadsFolder, webpFileName);
            using var webPImage = await Image.LoadAsync(image.OpenReadStream());
            await webPImage.SaveAsync(webpFilePath, new WebpEncoder { Quality = 75 });

            var request = contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/uploads/{webpFileName}";
        }
        public async Task<string> UploadVideo(IFormFile video, string instructorName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var sanitizedInstructorName = string.Join("_", instructorName.Split(Path.GetInvalidFileNameChars()));
            var fileExtention = Path.GetExtension(video.FileName).ToLower();

            var videoName = $"{sanitizedInstructorName}_{Guid.NewGuid()}{fileExtention}";
            var filePath = Path.Combine(uploadsFolder, videoName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await video.CopyToAsync(stream);
            }

            var request = contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/uploads/{videoName}";

        }
        public async Task<string> UploadPDF(IFormFile pdfFile, string instructorName)
        {
            var fileExtention = Path.GetExtension(pdfFile.FileName).ToLower();
            if (fileExtention != ".pdf")
            {
                throw new InvalidOperationException("Only PDF files are allowed");
            }
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var sanitizedInstructorName = string.Join("_", instructorName.Split(Path.GetInvalidFileNameChars()));

            var pdfName = $"{sanitizedInstructorName}_{Guid.NewGuid()}{fileExtention}";
            var filePath = Path.Combine(uploadsFolder, pdfName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream);
            }

            var request = contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/uploads/{pdfName}";

        }
        public async Task<string> UploadHtmlTemplate(IFormFile htmlFile, string templateTitle)
        {
            var fileExtension = Path.GetExtension(htmlFile.FileName).ToLower();
            if (fileExtension != ".html" && fileExtension != ".htm")
            {
                throw new InvalidOperationException("Only HTML files are allowed");
            }
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificate-templates");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var sanitizedTitle = string.Join("_", templateTitle.Split(Path.GetInvalidFileNameChars()));
            var htmlFileName = $"{sanitizedTitle}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, htmlFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await htmlFile.CopyToAsync(stream);
            }

            var request = contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/certificate-templates/{htmlFileName}";
        }
    }
}
