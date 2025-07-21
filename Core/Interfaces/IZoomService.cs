using Core.DTOs;

namespace Core.Interfaces
{
    public interface IZoomService
    {
        Task<ZoomMeetingDTO> CreateMeetingsAsync(CreateZoomMeetingDTO zoomMeetingDTO);
        Task UpdateMeetingAsync(UpdateZoomMeetingDTO zoomMeetingDTO);
        Task<ZoomMeetingDTO> GetMeetingByIdAsync(int meetingId);
        Task<List<ZoomMeetingDTO>> GetMeetingsByCourseIdAsync(int courseId);
        Task<List<ZoomMeetingDTO>> GetMeetingsByInstructorIdAsync(string instructorId);
        Task<List<ZoomMeetingDTO>> GetMeetingsByStudentIdAsync(string studentId);
    }
}
