using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetAuthController : ControllerBase
    {
        private readonly IGoogleMeetAuthService meetAuthService;
        public MeetAuthController(IGoogleMeetAuthService _meetAuthService)
        {
            meetAuthService = _meetAuthService;
        }
        [HttpGet("Authorize")]
        public IActionResult GetAuthorizationUrl()
        {
            var url = meetAuthService.GetAuthorizationUrlAsync();
            return Ok(new { url });
        }

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            {
                return BadRequest("Missing code or state (user ID).");
            }

            try
            {
                var account = await meetAuthService.HandleAuthCallbackAsync(code, state);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Authentication failed", details = ex.Message });
            }
        }
        
        [HttpGet("Status/{userId}")]
        public async Task<IActionResult> CheckConnectionStatus(string userId)
        {
            var account = await meetAuthService.GetUserConnectionAsync(userId);
            return Ok(new { isConnected = account != null && account.IsConnected });
        }

        // GET: api/GoogleAuth/Account/{userId}
        [HttpGet("Account/{userId}")]
        public async Task<IActionResult> GetAccount(string userId)
        {
            var account = await meetAuthService.GetUserConnectionAsync(userId);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }
    }
}
