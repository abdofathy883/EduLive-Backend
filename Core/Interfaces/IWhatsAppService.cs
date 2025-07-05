using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWhatsAppService
    {
        Task SendLessonReminderAsync(string toPhoneNumber, string reciverName, string courseTitle, DateTime lessonDateTime, string joinURL);
    }
}
