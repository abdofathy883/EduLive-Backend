using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        public ChatHub(IChatService service)
        {
            chatService = service;
        }
        public async Task SendMessageAsync(string reciverId, string message)
        {
            var senderid = Context.UserIdentifier
                ?? throw new ArgumentNullException(nameof(Context.UserIdentifier), "User identifier is null");

            await chatService.SendMessageAsync(senderid, reciverId, message);
            await Clients.User(reciverId).SendAsync("ReceiveMessage", senderid, message, DateTime.UtcNow);
        }
    }
}
