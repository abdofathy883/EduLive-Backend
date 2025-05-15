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
        Task<ZoomMeetingDTO> UpdateMeetingAsync(UpdateZoomMeetingDTO zoomMeetingDTO);
        Task<ZoomMeetingDTO> GetMeetingAsync(string meetingId);
        //Task<bool> DeleteMeetingAsync(string meetingId);
        //Task<ZoomMeetingResponse> GetMeetingDetailsAsync(string meetingId);
    }
}
