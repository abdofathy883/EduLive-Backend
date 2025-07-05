using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Background
{
    public class LessonReminderSchedular
    {
        private readonly IGenericRepo<Lesson> lessonRepo;
        private readonly IWhatsAppService whatsApp;

        public LessonReminderSchedular(
            IGenericRepo<Lesson> _lessonRepo,
            IWhatsAppService _whatsApp)
        {
            lessonRepo = _lessonRepo;
            whatsApp = _whatsApp;
        }

        public async Task SendRemindersForUpcomingLessonsAsync()
        {
            var utcNow = DateTime.UtcNow;
            var threshold = utcNow.AddMinutes(60);

            var lessons = await lessonRepo.FindAsync(
                l =>
                    (l.ZoomMeeting != null && l.ZoomMeeting.StartTime > utcNow && l.ZoomMeeting.StartTime <= threshold) ||
                    (l.GoogleMeetLesson != null && l.GoogleMeetLesson.StartTime > utcNow && l.GoogleMeetLesson.StartTime <= threshold),
                include: q => q
                    .Include(l => l.ZoomMeeting)
                    .Include(l => l.GoogleMeetLesson)
                    .Include(l => l.Instructor)
                    .Include(l => l.Student)
            );

            foreach (var lesson in lessons)
            {
                var meeting = lesson.ZoomMeeting ?? (object)lesson.GoogleMeetLesson;

                DateTime startTime;
                string joinUrl;

                if (meeting is ZoomMeeting zm)
                {
                    startTime = zm.StartTime;
                    joinUrl = zm.JoinUrl;
                }
                else if (meeting is GoogleMeetLesson gm)
                {
                    startTime = gm.StartTime ?? default;
                    joinUrl = gm.GoogleMeetURL;
                }
                else continue;

                string title = "your scheduled lesson";

                await whatsApp.SendLessonReminderAsync(
                    lesson.Student.PhoneNumber,
                    lesson.Student.FirstName,
                    title,
                    startTime,
                    joinUrl
                );

                await whatsApp.SendLessonReminderAsync(
                    lesson.Instructor.PhoneNumber,
                    lesson.Instructor.FirstName,
                    title,
                    startTime,
                    joinUrl
                );
            }
        }
    }
}
