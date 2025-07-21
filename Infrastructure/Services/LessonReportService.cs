using Core.DTOs;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class LessonReportService : ILessonReport
    {
        private readonly IGenericRepo<LessonReport> LessonReportRepo;
        public LessonReportService(IGenericRepo<LessonReport> repo)
        {
            LessonReportRepo = repo;
        }
        public async Task<LessonReport> AddReportAsync(LessonReportDTO newLesson)
        {
            var lessonReport = new LessonReport
            {
                MemorizedContent = newLesson.MemorizedContent,
                ExplainedTopics = newLesson.ExplainedTopics,
                DiscussedValues = newLesson.DiscussedValues,
                InstructorId = newLesson.InstructorId,
                StudentId = newLesson.StudentId
            };

            await LessonReportRepo.AddAsync(lessonReport);
            await LessonReportRepo.SaveAllAsync();
            return lessonReport;
        }

        public Task<List<LessonReportDTO>> GetReportsByInstructorIdAsync(string instructorId)
        {
            return LessonReportRepo.FindAsync(x => x.InstructorId == instructorId)
                .ContinueWith(task => task.Result.Select<LessonReport, LessonReportDTO>(MapLessonReportsDto).ToList());
        }
        public Task<List<LessonReportDTO>> GetReportsByStudentIdAsync(string studentId)
        {
            return LessonReportRepo.FindAsync(x => x.StudentId == studentId)
                .ContinueWith(task => task.Result.Select<LessonReport, LessonReportDTO>(MapLessonReportsDto).ToList());
        }

        private static LessonReportDTO MapLessonReportsDto(LessonReport m) // Corrected parameter type
        {
            return new LessonReportDTO
            {
                MemorizedContent = m.MemorizedContent,
                ExplainedTopics = m.ExplainedTopics,
                DiscussedValues = m.DiscussedValues,
                InstructorId = m.InstructorId,
                StudentId = m.StudentId
            };
        }

    }
}
