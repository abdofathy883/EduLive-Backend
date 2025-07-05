using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;
        public ChatController(IChatService _chatService)
        {
            chatService = _chatService;
        }
        
        [HttpGet("GetMessages/{otherUserId}")]
        public async Task<IActionResult> GetMessages(string otherUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(otherUserId))
            {
                return BadRequest("User ID or other user ID is missing.");
            }

            var messages = await chatService.GetConversationAsync(userId, otherUserId);
            return Ok(messages);
        }

        [HttpGet("instructors")]
        public async Task<IActionResult> GetAvailableInstructors()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var instructors = await chatService.GetAvailableInstructorsAsync(studentId);
            return Ok(instructors.Select(i => new {
                i.Id,
                i.FirstName,
                i.LastName
            }));
        }
    }
}
