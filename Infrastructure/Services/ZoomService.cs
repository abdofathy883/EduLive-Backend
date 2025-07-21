using Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Responses;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class ZoomService : IZoomService
    {
        private readonly HttpClient httpClient;
        private readonly IZoomAuthService zoomAuthService;
        private readonly IGenericRepo<Lesson> LessonRepo;
        private readonly IGenericRepo<ZoomMeeting> ZoomRepo;

        public ZoomService(HttpClient _httpClient, 
            IZoomAuthService zoomAuthService, 
            IGenericRepo<Lesson> genericRepo,
            IGenericRepo<ZoomMeeting> Repo)
        {
            httpClient = _httpClient;
            this.zoomAuthService = zoomAuthService;
            LessonRepo = genericRepo;
            ZoomRepo = Repo;
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
            if (zoomMeetingDTO == null)
                throw new ArgumentNullException(nameof(zoomMeetingDTO));

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
            var ZoomApiResponse = JsonSerializer.Deserialize<ZoomMeetingResponse>(responseContent)
                ?? throw new InvalidOperationException("Failed to deserialize Zoom meeting response");

            var NewLesson = new Lesson
            {
                Title = zoomMeetingDTO.Topic,
                Description = zoomMeetingDTO.Description,
                LessonPlatform = Core.Enums.LessonPlatform.Zoom,
                CourseId = zoomMeetingDTO.CourseId,
                InstructorId = zoomMeetingDTO.InstructorId,
                StudentId = zoomMeetingDTO.StudentId,
            };
            // Save to database
            
            await LessonRepo.AddAsync(NewLesson);
            await LessonRepo.SaveAllAsync();

            var NewZoomMeeting = new ZoomMeeting
            {
                LessonId = NewLesson.LessonId,
                ZoomMeetingId = ZoomApiResponse.Id,
                StartTime = zoomMeetingDTO.StartTime,
                Duration = zoomMeetingDTO.Duration,
                JoinUrl = ZoomApiResponse.JoinUrl,
                StartUrl = ZoomApiResponse.start_url,
                Password = ZoomApiResponse.password
            };

            await ZoomRepo.AddAsync(NewZoomMeeting);
            await ZoomRepo.SaveAllAsync();

            return MapLessonToZoomDto(NewLesson);
        }

        public async Task<ZoomMeetingDTO> GetMeetingByIdAsync(int meetingId)
        {
            var lesson = await LessonRepo.GetByIdAsync(meetingId)
                ?? throw new ArgumentException("Zoom meeting not found");
            if (lesson.LessonPlatform != LessonPlatform.Zoom)
                throw new ArgumentException("Lesson is not a Zoom meeting");

            if (lesson.IsDeleted)
                throw new ArgumentException("This lesson has been deleted");

            if (lesson.ZoomMeetingId == null)
                throw new ArgumentException("This lesson does not have a Zoom meeting associated with it");

            var ZoomLesson = await ZoomRepo.GetByIdAsync(lesson.ZoomMeetingId)
                ?? throw new ArgumentException("Zoom meeting not found for this lesson");

            return MapLessonToZoomDto(lesson);
        }

        public async Task UpdateMeetingAsync(UpdateZoomMeetingDTO zoomMeetingDTO)
        {
            var ZoomLesson = await LessonRepo.GetByIdAsync(zoomMeetingDTO.LessonId)
                ?? throw new ArgumentException("Lesson not found");

            if (ZoomLesson.LessonPlatform != LessonPlatform.Zoom)
                throw new ArgumentException("This lesson is not a Zoom meeting");

            if (ZoomLesson.IsDeleted)
                throw new ArgumentException("This lesson has been deleted");

            if (ZoomLesson.ZoomMeetingId == null)
                throw new ArgumentException("This lesson does not have a Zoom meeting associated with it");

            var ZoomMeeting = await ZoomRepo.GetByIdAsync(ZoomLesson.ZoomMeetingId)
                ?? throw new ArgumentException("Zoom meeting not found");

            var accessToken = await zoomAuthService.RefreshAccessTokenAsync(ZoomLesson.InstructorId);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var updateRequest = new
            {
                topic = zoomMeetingDTO.Topic,
                start_time = zoomMeetingDTO.StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = zoomMeetingDTO.Duration,
                password = zoomMeetingDTO.Password
            };

            var content = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync($"https://api.zoom.us/v2/meetings/{ZoomMeeting.ZoomMeetingId}", content);
            response.EnsureSuccessStatusCode();

            ZoomMeeting.StartTime = zoomMeetingDTO.StartTime;
            ZoomMeeting.Duration = zoomMeetingDTO.Duration;
            ZoomMeeting.Password = zoomMeetingDTO.Password;
            ZoomMeeting.StartTime = zoomMeetingDTO.StartTime;
            ZoomMeeting.Duration = zoomMeetingDTO.Duration;
            ZoomLesson.Title = zoomMeetingDTO.Topic;

            await ZoomRepo.SaveAllAsync();
            await LessonRepo.SaveAllAsync();
        }

        public async Task<List<ZoomMeetingDTO>> GetMeetingsByCourseIdAsync(int courseId)
        {
            var ZoomMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == LessonPlatform.Zoom);
            return ZoomMeetings
                .Where(m => m.CourseId == courseId && 
                            !m.IsDeleted && m.ZoomMeeting != null)
                .Select(MapLessonToZoomDto).ToList();
        }

        public async Task<List<ZoomMeetingDTO>> GetMeetingsByInstructorIdAsync(string instructorId)
        {
            var ZoomMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == LessonPlatform.Zoom);
            return ZoomMeetings
                .Where(m => m.InstructorId == instructorId &&
                            !m.IsDeleted && m.ZoomMeeting != null)
                .Select(MapLessonToZoomDto).ToList();
        }

        public async Task<List<ZoomMeetingDTO>> GetMeetingsByStudentIdAsync(string studentId)
        {
            var ZoomMeetings = await LessonRepo.FindAsync(m => m.LessonPlatform == LessonPlatform.Zoom);
            return ZoomMeetings
                .Where(m => m.StudentId == studentId && 
                            !m.IsDeleted && m.ZoomMeeting != null)
                .Select(MapLessonToZoomDto).ToList();
        }

        private static ZoomMeetingDTO MapLessonToZoomDto(Lesson m)
        {
            return new ZoomMeetingDTO
            {
                LessonId = m.LessonId,
                ZoomMeetingId = m.ZoomMeeting.ZoomMeetingId,
                Topic = m.Title,
                StartTime = m.ZoomMeeting?.StartTime ?? default,
                Duration = m.ZoomMeeting?.Duration ?? 0,
                JoinUrl = m.ZoomMeeting.JoinUrl,
                StartUrl = m.ZoomMeeting.StartUrl,
                Password = m.ZoomMeeting.Password,
                CourseId = m.CourseId,
                InstructorId = m.InstructorId,
                StudentId = m.StudentId
            };
        }

    }
}
