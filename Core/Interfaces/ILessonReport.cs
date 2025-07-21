using Core.DTOs;
using Core.Models;

namespace Core.Interfaces
{
    public interface ILessonReport
    {
        Task<LessonReport> AddReportAsync(LessonReportDTO newLesson);
        Task<List<LessonReportDTO>> GetReportsByStudentIdAsync(string studentId);
        Task<List<LessonReportDTO>> GetReportsByInstructorIdAsync(string instructorId);
    }
}
