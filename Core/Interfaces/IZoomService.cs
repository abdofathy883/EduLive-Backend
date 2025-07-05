using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
