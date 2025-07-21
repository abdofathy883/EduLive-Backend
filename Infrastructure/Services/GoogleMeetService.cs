using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null");

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

            await LessonRepo.AddAsync(NewLesson);
            await LessonRepo.SaveAllAsync();

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

            return MapLessonToMeetDto(NewLesson);
        }

        public async Task<GoogleMeetMeetingDTO> GetMeetingByIdAsync(int meetingId)
        {
            if (meetingId <= 0)
                throw new ArgumentOutOfRangeException(nameof(meetingId), "Meeting ID must be greater than zero");

            var lesson = await LessonRepo.GetByIdAsync(meetingId)
                ?? throw new ArgumentException("Lesson not found for the given meeting ID");

            if (lesson.GoogleMeetId == null)
                throw new ArgumentException("This lesson is not a Google Meet lesson");

            var MeetLesson = await MeetRepo.GetByIdAsync(lesson.GoogleMeetId)
                ?? throw new ArgumentException("Google Meet lesson not found");

            return MapLessonToMeetDto(lesson);
        }

        public async Task<List<GoogleMeetMeetingDTO>> GetMeetingsByCourseIdAsync(int courseId)
        {
            var GoogleMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == Core.Enums.LessonPlatform.GoogleMeet);
            return GoogleMeetings
                .Where(m => m.CourseId == courseId && !m.IsDeleted && m.GoogleMeetLesson != null)
                .Select(MapLessonToMeetDto)
                .ToList();
        }

        public async Task<List<GoogleMeetMeetingDTO>> GetMeetingsByInstructorIdAsync(string instructorId)
        {
            var GoogleMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == Core.Enums.LessonPlatform.GoogleMeet);
            return GoogleMeetings
                .Where(m => m.InstructorId == instructorId && !m.IsDeleted && m.GoogleMeetLesson != null)
                .Select(MapLessonToMeetDto)
                .ToList();
        }

        public async Task<List<GoogleMeetMeetingDTO>> GetMeetingsByStudentIdAsync(string studentId)
        {
            var GoogleMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == Core.Enums.LessonPlatform.GoogleMeet);
            return GoogleMeetings
                .Where(m => m.StudentId == studentId && !m.IsDeleted && m.GoogleMeetLesson != null)
                .Select(MapLessonToMeetDto)
                .ToList();
        }

        public Task<GoogleMeetMeetingDTO> UpdateMeetingAsync(int lessonId, UpdateGoogleMeetMeetingDTO request)
        {
            throw new NotImplementedException();
        }

        private static GoogleMeetMeetingDTO MapLessonToMeetDto(Lesson m)
        {
            return new GoogleMeetMeetingDTO
            {
                LessonId = m.LessonId,
                GoogleEventId = m.GoogleMeetLesson?.GoogleEventId ?? string.Empty,
                Topic = m.Title,
                StartTime = m.GoogleMeetLesson?.StartTime ?? default,
                Duration = m.GoogleMeetLesson?.Duration ?? 0,
                GoogleMeetURL = m.GoogleMeetLesson?.GoogleMeetURL ?? string.Empty,
                CourseId = m.CourseId,
                InstructorId = m.InstructorId,
                StudentId = m.StudentId
            };
        }

        
    }
}
