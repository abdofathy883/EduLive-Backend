using Core.DTOs;

namespace Core.Interfaces
{
    public interface ICertificateGenerationService
    {
        Task<string> GenerateCertificatePdfAsync(int templateId, string studentId, string instructorId, int courseId);
        Task<List<CertificateDTO>> GetStudentCertificatesAsync(string studentId);
        Task<byte[]> GetCertificatePdfBytesAsync(string serialNumber);
    }
}
