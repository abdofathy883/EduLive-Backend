using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Infrastructure.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly string accountId;
        private readonly string authToken;
        private readonly string fromPhoneNumber;

        private readonly ILogger<WhatsAppService> logger;
        private readonly E_LearningDbContext dbContext;

        public WhatsAppService(ILogger<WhatsAppService> _logger, E_LearningDbContext _dbContext)
        {
            logger = _logger;
            dbContext = _dbContext;
        }
        public async Task<bool> SendLessonAlertAsync(int lessonId, string userId)
        {
            //TwilioClient.Init(accountId, authToken);

            //foreach (var PhoneNumber in toPhoneNumbers)
            //{
            //    await MessageResource.CreateAsync(
            //        to: new Twilio.Types.PhoneNumber(PhoneNumber),
            //        from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
            //        body: message
            //    );
            //}

            try
            {
                var lesson = await dbContext.Lessons.FirstOrDefaultAsync(l => l.LessonId == lessonId);
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (lesson is null || user is null || string.IsNullOrEmpty(user.PhoneNumber))
                {
                    logger.LogWarning($"Cannot send reminder for lesson {lessonId} to user {userId}");
                    return false;
                }

                //TODO: looking in Role point
                string message = "";
                //string message = user.Role == UserRole.Student
                //    ? $"Reminder: Your class '{lesson.Title}' starts in 30 minutes at {lesson.StartTime.ToString("HH:mm")}. Please be prepared."
                //    : $"Reminder: You'll be teaching '{lesson.Title}' in 30 minutes at {lesson.StartTime.ToString("HH:mm")}. Your students are waiting!";

                bool result = await SendMessageAsync(user.PhoneNumber, message);

                var notification = new Notification
                {
                    //UserId = userId.ToString(),
                    LessonId = lessonId,
                    SchudledTime = DateTime.UtcNow.AddMinutes(-1),
                    ActualSentTime = result ? DateTime.UtcNow : null,
                    Status = result ? NotificationStatus.Sent : NotificationStatus.Failed,
                    MessageContent = message
                };

                dbContext.Notifications.Add(notification);
                await dbContext.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error sending lesson alert fro lesson {lessonId}, to user {userId}");
                return false;
            }
        }

        public async Task<bool> SendMessageAsync(string phoneNumber, string message)
        {
            try
            {
                string WhatsAppNumber = phoneNumber.StartsWith("whatsapp") ? phoneNumber : $"whatsapp:{phoneNumber}";
                string fromNumber = fromPhoneNumber; // will be changed to env var

                var messageResources = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(fromNumber),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                    );
                logger.LogInformation($"WhatsApp message sent to {phoneNumber}, SID: {messageResources.Sid}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to send WhatsApp message to {phoneNumber}");
                return false;
            }
        }

        public async Task SendSchudledRemindersAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                int value = 25;
                var upcomingLessons = await dbContext.Lessons
                    .Where(l => l.Date_Time > now.AddMinutes(25)).Include(l => l.InstructorId)
                    .Include(l => l.StudentId).ToListAsync();

                foreach (var lesson in upcomingLessons)
                {
                    //add schudeled time
                    var existingNotofication = await dbContext.Notifications
                        .Where(n => n.LessonId == lesson.LessonId && (n.Status == NotificationStatus.Sent || n.Status == NotificationStatus.Pending)).AnyAsync();
                    if (existingNotofication)
                    {
                        logger.LogInformation($"Notification for lesson {lesson.LessonId} already sent or pending");
                        continue;
                    }
                    if (lesson.Instructor != null && !string.IsNullOrEmpty(lesson.Instructor.PhoneNumber))
                    {
                        await SendLessonAlertAsync(lesson.LessonId, lesson.Instructor.Id);
                    }
                    //used only if business requiere one student in the lesson
                    if (lesson.Student != null && !string.IsNullOrEmpty(lesson.Student.PhoneNumber))
                    {
                        await SendLessonAlertAsync(lesson.LessonId, lesson.Student.Id);
                    }
                    // used only if business requieres list of students in the lesson
                    //foreach (var student in lesson.Students.Where(s => s.IsActive && !string.IsNullOrEmpty(s.PhoneNumber)))
                    //{
                    //    await SendLessonAlertAsync(lesson.LessonId, student.Id);
                    //}

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing schudeled reminders");
            }
        }
    }
}
