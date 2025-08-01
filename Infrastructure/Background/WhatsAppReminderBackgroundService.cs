﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Background
{
    public class WhatsAppReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<WhatsAppReminderBackgroundService> logger;
        private readonly IServiceProvider serviceProvider;

        public WhatsAppReminderBackgroundService(ILogger<WhatsAppReminderBackgroundService> _logger, IServiceProvider _serviceProvider)
        {
            logger = _logger;
            serviceProvider = _serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var schedular = scope.ServiceProvider.GetRequiredService<LessonReminderSchedular>();

                await schedular.SendRemindersForUpcomingLessonsAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
