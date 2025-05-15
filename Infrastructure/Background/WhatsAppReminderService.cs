using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Background
{
    public class WhatsAppReminderService : BackgroundService
    {
        private readonly ILogger<WhatsAppReminderService> logger;
        private readonly IServiceProvider serviceProvider;

        public WhatsAppReminderService(ILogger<WhatsAppReminderService> _logger, IServiceProvider _serviceProvider)
        {
            logger = _logger;
            serviceProvider = _serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("WhatsApp Reminder Service Starting");
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();
                    //await whatsAppService.SendScheduledRemindersAsync();
                } 
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing WhatsApp reminders");
            }
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
