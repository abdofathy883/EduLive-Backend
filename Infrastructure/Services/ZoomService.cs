using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ZoomService : IZoomService
    {
        private readonly HttpClient httpClient;
        private readonly IZoomAuthService zoomAuthService;
        private readonly IGenericRepo<ZoomMeeting> repo;

        public ZoomService(HttpClient _httpClient, IZoomAuthService zoomAuthService, IGenericRepo<ZoomMeeting> genericRepo)
        {
            httpClient = _httpClient;
            this.zoomAuthService = zoomAuthService;
            repo = genericRepo;
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<ZoomMeetingDTO> CreateMeetingsAsync(CreateZoomMeetingDTO zoomMeetingDTO)
        {
            var userId = zoomMeetingDTO.InstructorId;
            var accessToken = await zoomAuthService.RefreshAccessTokenAsync(userId);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var meetingRequest = new
            {
                topic = zoomMeetingDTO.Topic,
                type = 2, // Scheduled meeting
                start_time = zoomMeetingDTO.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = zoomMeetingDTO.Duration,
                timezone = "UTC",
                password = GenerateRandomPassword(),
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = true,
                    mute_upon_entry = true,
                    waiting_room = false,
                    meeting_authentication = false
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(meetingRequest),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(
                $"https://api.zoom.us/v2/users/me/meetings",
                content);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var zoomMeeting = JsonSerializer.Deserialize<ZoomMeetingResponse>(responseContent);

            // Save to database
            var meeting = new ZoomMeeting
            {
                Id = Guid.NewGuid(),
                ZoomMeetingId = zoomMeeting.Id,
                Topic = zoomMeetingDTO.Topic,
                StartTime = zoomMeetingDTO.StartTime,
                Duration = zoomMeetingDTO.Duration,
                JoinUrl = zoomMeeting.JoinUrl,
                StartUrl = zoomMeeting.start_url,
                Password = zoomMeeting.password,
                CourseId = zoomMeetingDTO.CourseId,
                InstructorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(meeting);
            await repo.SaveAllAsync();

            var resultDto = new ZoomMeetingDTO
            {
                Id = meeting.Id,
                ZoomMeetingId = meeting.ZoomMeetingId,
                Topic = meeting.Topic,
                StartTime = meeting.StartTime,
                Duration = meeting.Duration,
                JoinUrl = meeting.JoinUrl,
                StartUrl = meeting.StartUrl,
                Password = meeting.Password,
                CourseId = meeting.CourseId,
                InstructorId = meeting.InstructorId,
            };
            return resultDto;
        }

        public Task<ZoomMeetingDTO> GetMeetingAsync(string meetingId)
        {
            throw new NotImplementedException();
        }

        public Task<ZoomMeetingDTO> UpdateMeetingAsync(UpdateZoomMeetingDTO zoomMeetingDTO)
        {
            throw new NotImplementedException();
        }
    }
}
