using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZoomAuthController : ControllerBase
    {
        private readonly IZoomAuthService authService;
        public ZoomAuthController(IZoomAuthService zoomAuthService)
        {
            authService = zoomAuthService;
        }
        [HttpGet("connect")]
        public IActionResult GetAuthorizationUrl()
        {
            var url = authService.GetAuthorizationUrl();
            return Ok(new { url });
        }
        [HttpGet("callback")]
        public async Task<IActionResult> HandleCallback([FromQuery] string code, [FromQuery] Guid state)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing Zoom authorization code");

            var result = await authService.HandleOAuthCallback(code, state);
            return Ok(result);
        }
        [HttpGet("status/{userId}")]
        public async Task<IActionResult> GetConnectionStatus(Guid userId)
        {
            var connection = await authService.GetUserConnectionAsync(userId);
            return Ok(connection);
        }
        [HttpPost("refresh/{userId}")]
        public async Task<IActionResult> RefreshToken(Guid userId)
        {
            try
            {
                var newAccessToken = await authService.RefreshAccessTokenAsync(userId);
                return Ok(new { accessToken = newAccessToken });
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
        [HttpPost("revoke/{userId}")]
        public async Task<IActionResult> RevokeAccess(Guid userId)
        {
            var result = await authService.RevokeAccessAsync(userId);
            return Ok(new { revoked = result });
        }
    }
}
