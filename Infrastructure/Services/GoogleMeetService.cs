using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class GoogleMeetService : IGoogleMeetService
    {
        private readonly IGenericRepo<GoogleMeetLesson> MeetRepo;
        private readonly IGenericRepo<Lesson> LessonRepo;
        private readonly IGoogleMeetAuthService authService;
        private readonly IHttpClientFactory httpClient;
        private readonly ILogger<GoogleMeetService> logger;
        public GoogleMeetService(IGenericRepo<GoogleMeetLesson> generic, 
            IHttpClientFactory clientFactory, 
            ILogger<GoogleMeetService> _logger,
            IGoogleMeetAuthService googleMeet,
            IGenericRepo<Lesson> genericRepo)
        {
            MeetRepo = generic;
            httpClient = clientFactory;
            logger = _logger;
            authService = googleMeet;
            LessonRepo = genericRepo;
        }

        public async Task<GoogleMeetMeetingDTO> CreateMeetingAsync(CreateGoogleMeetMeetingDTO request)
        {
            var accessToken = await authService.GetAccessTokenAsync(request.InstructorId);

            var client = httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var calendarEvent = new
            {
                summary = request.Title,
                description = request.Description,
                start = new { dateTime = request.StartTime.ToString("o"), timeZone = "UTC" },
                conferenceData = new
                {
                    createRequest = new
                    {
                        requestId = Guid.NewGuid().ToString(),
                        conferenceSolutionKey = new { type = "hangoutsMeet" }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(calendarEvent), Encoding.UTF8, "application/json");

            var url = "https://www.googleapis.com/calendar/v3/calendars/primary/events?conferenceDataVersion=1";
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(responseBody);

            var meetingLink = result.RootElement.GetProperty("conferenceData")
                .GetProperty("entryPoints")[0].GetProperty("uri").GetString();
            var eventId = result.RootElement.GetProperty("id").GetString();

            var NewLesson = new Lesson
            {
                Title = request.Title,
                Description = request.Description,
                LessonPlatform = Core.Enums.LessonPlatform.GoogleMeet,
                InstructorId = request.InstructorId,
                StudentId = request.StudentId,
                CourseId = request.CourseId,

            };

            var newMeeting = new GoogleMeetLesson
            {
                LessonId = NewLesson.LessonId,
                GoogleEventId = eventId,
                GoogleCalendarId = "primary",
                GoogleMeetURL = meetingLink,
                StartTime = request.StartTime,
                Duration = request.Duration,
            };

            await MeetRepo.AddAsync(newMeeting);
            await MeetRepo.SaveAllAsync();

            return new GoogleMeetMeetingDTO
            {
                Topic = NewLesson.Title,
                GoogleEventId = newMeeting.GoogleEventId,
                GoogleMeetURL = newMeeting.GoogleMeetURL,
                StartTime = newMeeting.StartTime ?? default,
                Duration = newMeeting.Duration,
            };
        }

        public async Task<GoogleMeetMeetingDTO> GetMeetingByIdAsync(int meetingId)
        {
            var lesson = await LessonRepo.GetByIdAsync(meetingId);
            var MeetLesson = await MeetRepo.GetByIdAsync(lesson.ZoomMeetingId);

            if (lesson == null)
                throw new ArgumentException("Zoom meeting not found");

            return new GoogleMeetMeetingDTO
            {
                GoogleEventId = MeetLesson.GoogleEventId,
                Topic = lesson.Title,
                StartTime = MeetLesson.StartTime ?? default,
                Duration = MeetLesson.Duration,
                GoogleMeetUrl = MeetLesson.GoogleMeetURL,
                CourseId = lesson.CourseId,
                InstructorId = lesson.InstructorId,
                StudentId = lesson.StudentId,
                LessonId = lesson.LessonId
            };
        }

        public async Task<List<GoogleMeetMeetingDTO>> GetMeetingsByCourseIdAsync(int courseId)
        {
            var GoogleMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == Core.Enums.LessonPlatform.GoogleMeet);
            return GoogleMeetings
                .Where(m => m.CourseId == courseId && !m.IsDeleted && m.GoogleMeetLesson != null)
                .Select(MapLessonToZoomDto)
                .ToList();
        }

        public async Task<List<GoogleMeetMeetingDTO>> GetMeetingsByInstructorIdAsync(string instructorId)
        {
            var GoogleMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == Core.Enums.LessonPlatform.GoogleMeet);
            return GoogleMeetings
                .Where(m => m.InstructorId == instructorId && !m.IsDeleted && m.GoogleMeetLesson != null)
                .Select(MapLessonToZoomDto)
                .ToList();
        }

        public async Task<List<GoogleMeetMeetingDTO>> GetMeetingsByStudentIdAsync(string studentId)
        {
            var GoogleMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == Core.Enums.LessonPlatform.GoogleMeet);
            return GoogleMeetings
                .Where(m => m.StudentId == studentId && !m.IsDeleted && m.GoogleMeetLesson != null)
                .Select(MapLessonToZoomDto)
                .ToList();
        }

        public Task<GoogleMeetMeetingDTO> UpdateMeetingAsync(int lessonId, UpdateGoogleMeetMeetingDTO request)
        {
            throw new NotImplementedException();
        }

        private static GoogleMeetMeetingDTO MapLessonToZoomDto(Lesson m)
        {
            return new GoogleMeetMeetingDTO
            {
                LessonId = m.LessonId,
                GoogleEventId = m.GoogleMeetLesson?.GoogleEventId,
                Topic = m.Title,
                StartTime = m.GoogleMeetLesson?.StartTime ?? default,
                Duration = m.GoogleMeetLesson?.Duration ?? 0,
                GoogleMeetURL = m.GoogleMeetLesson?.GoogleMeetURL,
                CourseId = m.CourseId,
                InstructorId = m.InstructorId,
                StudentId = m.StudentId
            };
        }

        
    }
}
