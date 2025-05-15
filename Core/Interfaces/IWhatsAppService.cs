using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWhatsAppService
    {
        Task<bool> SendMessageAsync(string phoneNumber, string message);
        Task<bool> SendLessonAlertAsync(int lessonId, string userId);
        Task SendSchudledRemindersAsync();
    }
}
