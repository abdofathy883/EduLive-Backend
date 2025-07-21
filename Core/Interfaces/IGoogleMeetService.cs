using Core.DTOs;

namespace Core.Interfaces
{
    public interface IGoogleMeetService
    {
        Task<GoogleMeetMeetingDTO> CreateMeetingAsync(CreateGoogleMeetMeetingDTO request);
        Task<GoogleMeetMeetingDTO> UpdateMeetingAsync(int lessonId, UpdateGoogleMeetMeetingDTO request);
        Task<GoogleMeetMeetingDTO> GetMeetingByIdAsync(int meetingId);
        Task<List<GoogleMeetMeetingDTO>> GetMeetingsByCourseIdAsync(int courseId);
        Task<List<GoogleMeetMeetingDTO>> GetMeetingsByInstructorIdAsync(string instructorId);
        Task<List<GoogleMeetMeetingDTO>> GetMeetingsByStudentIdAsync(string studentId);
    }
}
