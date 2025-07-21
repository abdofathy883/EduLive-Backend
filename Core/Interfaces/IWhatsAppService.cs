namespace Core.Interfaces
{
    public interface IWhatsAppService
    {
        Task SendLessonReminderAsync(string toPhoneNumber, string reciverName, string courseTitle, DateTime lessonDateTime, string joinURL);
    }
}
