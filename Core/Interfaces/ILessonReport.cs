using Core.DTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILessonReport
    {
        Task<LessonReport> AddReportAsync(LessonReportDTO newLesson);
        Task<List<LessonReportDTO>> GetReportsByStudentIdAsync(string studentId);
        Task<List<LessonReportDTO>> GetReportsByInstructorIdAsync(string instructorId);
    }
}
