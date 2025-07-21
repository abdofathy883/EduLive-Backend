using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Twilio.Rest.Api.V2010.Account;

namespace Infrastructure.Services
{
    public class WhatsAppService : IWhatsAppService
    {

        private readonly ILogger<WhatsAppService> logger;
        private readonly IOptions<TwilioSettings> twilioSettings;

        // Simple in-memory rate limiter: phone -> list of send times
        private static readonly ConcurrentDictionary<string, List<DateTime>> _rateLimitDict = new();

        private const int RATE_LIMIT = 3; // max messages
        private static readonly TimeSpan RATE_LIMIT_WINDOW = TimeSpan.FromMinutes(1);

        public WhatsAppService(
            ILogger<WhatsAppService> _logger,
            IOptions<TwilioSettings> options
            )
        {
            logger = _logger;
            twilioSettings = options;
        }

        public async Task SendLessonReminderAsync(string toPhoneNumber, string reciverName, string courseTitle, DateTime lessonDateTime, string joinURL)
        {
            var now = DateTime.UtcNow;
            var timestamps = _rateLimitDict.GetOrAdd(toPhoneNumber, _ => new List<DateTime>());
            lock (timestamps)
            {
                timestamps.RemoveAll(t => (now - t) > RATE_LIMIT_WINDOW);
                if (timestamps.Count >= RATE_LIMIT)
                {
                    logger.LogWarning($"Rate limit exceeded for {toPhoneNumber}. Message not sent.");
                    return;
                }
                timestamps.Add(now);
            }

            string MessageBody = $"Hello {reciverName},\n" +
                $"This is a reminder for your upcoming lesson in the course '{courseTitle}'.\n" +
                $"Lesson Date and Time: {lessonDateTime.ToString("f")}\n" +
                $"Join URL: {joinURL}";

            int maxRetries = 3;
            int delayMs = 1000;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var result = await MessageResource.CreateAsync(
                        body: MessageBody,
                        from: new Twilio.Types.PhoneNumber(twilioSettings.Value.WhatsappFromNumber), // Twilio WhatsApp number
                        to: new Twilio.Types.PhoneNumber($"whatsapp:{toPhoneNumber}") // Recipient's WhatsApp number
                    );
                    logger.LogInformation($"WhatsApp message sent successfully to {toPhoneNumber}. Message SID: {result.Sid}");
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Attempt {attempt}: Failed to send WhatsApp message to {toPhoneNumber}. Error: {ex.Message}");
                    if (attempt == maxRetries)
                        throw;
                    await Task.Delay(delayMs);
                    delayMs *= 2; // Exponential backoff
                }
            }
        }
    }
}
