using Core.DTOs;
using Core.Enums;
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
        private readonly IGenericRepo<Lesson> repo;

        public ZoomService(HttpClient _httpClient, IZoomAuthService zoomAuthService, IGenericRepo<Lesson> genericRepo)
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

            var meeting = new Lesson
            {
                Title = zoomMeetingDTO.Topic,
                Description = zoomMeetingDTO.Description,
                Date_Time = zoomMeetingDTO.StartTime,
                Duration = zoomMeetingDTO.Duration,
                LessonPlatform = Core.Enums.LessonPlatform.Zoom,
                CourseId = zoomMeetingDTO.CourseId,
                InstructorId = zoomMeetingDTO.InstructorId,
                StudentId = zoomMeetingDTO.StudentId,
                ZoomMeetingId = zoomMeeting.Id,
                ZoomJoinURL = zoomMeeting.JoinUrl,
                ZoomStartUrl = zoomMeeting.start_url,
                ZoomPassword = zoomMeeting.password,
            };
            // Save to database
            

            await repo.AddAsync(meeting);
            await repo.SaveAllAsync();

            var resultDto = new ZoomMeetingDTO
            {
                Id = meeting.LessonId,
                ZoomMeetingId = meeting.ZoomMeetingId,
                Topic = meeting.Title,
                StartTime = meeting.Date_Time,
                Duration = meeting.Duration,
                JoinUrl = meeting.ZoomJoinURL,
                StartUrl = meeting.ZoomStartUrl,
                Password = meeting.ZoomPassword,
                CourseId = meeting.CourseId,
                InstructorId = meeting.InstructorId,
                StudentId = meeting.StudentId

            };
            return resultDto;
        }

        public async Task<ZoomMeetingDTO> GetMeetingAsync(string meetingId)
        {
            var lessons = await repo.GetAllAsync(); // Await the Task to get the actual list
            var lesson = lessons
                .Where(l => l.ZoomMeetingId == meetingId && l.LessonPlatform == LessonPlatform.Zoom)
                .FirstOrDefault();

            if (lesson == null)
                throw new ArgumentException("Zoom meeting not found");

            return new ZoomMeetingDTO
            {
                Id = lesson.LessonId,
                ZoomMeetingId = lesson.ZoomMeetingId,
                Topic = lesson.Title,
                StartTime = lesson.Date_Time,
                Duration = lesson.Duration,
                JoinUrl = lesson.ZoomJoinURL,
                StartUrl = lesson.ZoomStartUrl,
                Password = lesson.ZoomPassword,
                CourseId = lesson.CourseId,
                InstructorId = lesson.InstructorId
            };
        }

        public Task<ZoomMeetingDTO> UpdateMeetingAsync(UpdateZoomMeetingDTO zoomMeetingDTO)
        {
            throw new NotImplementedException();
        }
    }
}
