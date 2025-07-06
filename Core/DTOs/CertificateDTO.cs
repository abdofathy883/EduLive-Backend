namespace Core.DTOs
{
    public class CertificateDTO
    {
        public string SerialNumber { get; set; }
        public string CourseName { get; set; }
        public string TemplateTitle { get; set; }
        public string PdfDownloadUrl { get; set; }
        public DateOnly IssueDate { get; set; }
    }
}
