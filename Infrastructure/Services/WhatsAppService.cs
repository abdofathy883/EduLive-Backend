using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        private readonly ILogger<WhatsAppService> logger;
        private readonly IGenericRepo<Lesson> lessonRepo;
        private readonly IOptions<TwilioSettings> twilioSettings;


        public WhatsAppService(ILogger<WhatsAppService> _logger, 
            IGenericRepo<Lesson> genericRepo,
            IOptions<TwilioSettings> options)
        {
            logger = _logger;
            lessonRepo = genericRepo;
            twilioSettings = options;
        }

        public async Task SendLessonReminderAsync(string toPhoneNumber, string reciverName, string courseTitle, DateTime lessonDateTime, string joinURL)
        {
            try
            {
                string MessageBody = $"Hello {reciverName},\n" +
                    $"This is a reminder for your upcoming lesson in the course '{courseTitle}'.\n" +
                    $"Lesson Date and Time: {lessonDateTime.ToString("f")}\n" +
                    $"Join URL: {joinURL}";

                var result = await MessageResource.CreateAsync(
                    body: MessageBody,
                    from: new Twilio.Types.PhoneNumber(twilioSettings.Value.WhatsappFromNumber), // Twilio WhatsApp number
                    to: new Twilio.Types.PhoneNumber($"whatsapp:{toPhoneNumber}") // Recipient's WhatsApp number
                );
                logger.LogInformation($"WhatsApp message sent successfully to {toPhoneNumber}. Message SID: {result.Sid}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to send WhatsApp message to {toPhoneNumber}. Error: {ex.Message}");
            }
        }
    }
}
