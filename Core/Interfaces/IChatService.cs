using Core.Models;

namespace Core.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<BaseUser>> GetAvailableInstructorsAsync(string studentId);
        Task SendMessageAsync(string senderId, string receiverId, string messageContent);
        Task<IEnumerable<Message>> GetConversationAsync(string user1Id, string user2Id);
    }
}
