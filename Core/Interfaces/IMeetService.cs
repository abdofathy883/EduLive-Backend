using Core.DTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMeetService
    {
        Task<GoogleMeetLessonDTO> CreateMeetingAsync(CreateGoogleMeetRequest request, int lessonId);
        Task<GoogleMeetLessonDTO> GetMeetingByLessonIdAsync(int lessonId);
        Task DeleteMeetingAsync(int lessonId);
        Task<string> GenerateMeetingJoinUrlAsync(int lessonId);
    }
}
