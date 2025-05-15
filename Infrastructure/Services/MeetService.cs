using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class MeetService : IMeetService
    {
        private readonly IGenericRepo<GoogleMeetLesson> repo;
        private readonly IHttpClientFactory httpClient;
        private readonly ILogger<MeetService> logger;
        public MeetService(IGenericRepo<GoogleMeetLesson> generic, IHttpClientFactory clientFactory, ILogger<MeetService> _logger)
        {
            repo = generic;
            httpClient = clientFactory;
            logger = _logger;
        }
        public Task<GoogleMeetLessonDTO> CreateMeetingAsync(CreateGoogleMeetRequest request, int lessonId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMeetingAsync(int lessonId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateMeetingJoinUrlAsync(int lessonId)
        {
            throw new NotImplementedException();
        }

        public Task<GoogleMeetLessonDTO> GetMeetingByLessonIdAsync(int lessonId)
        {
            throw new NotImplementedException();
        }
    }
}
