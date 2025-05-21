using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth authService;
        public AuthController(IAuth _authService)
        {
            authService = _authService;
        }

        private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOption);
        }

        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { Message = "No refresh token provided" });
            }
            var result = await authService.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new { result.Message });
            }
            if (result.RefreshToken is not null)
            {
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await authService.LoginAsync(loginDTO);
            if (!result.IsAuthenticated)
            {
                return BadRequest(new { result.Message });
            }
            if (result.RefreshToken is not null)
            {
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var results = await authService.RegisterAsync(registerDTO);
            if (!results.IsAuthenticated)
            {
                return BadRequest(new { results.Message });
            }
            return Ok(new { results.Message });
        }
        [HttpPost("register-instructor")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterInstructor([FromForm] InstructorRegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var results = await authService.InstructorRegisterAsync(registerDTO);
            if (!results.IsAuthenticated)
            {
                return BadRequest(new { results.Message });
            }
            return Ok(new { results.Message });
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(string userId, [FromForm] UpdateUserDTO updateUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await authService.UpdateUserAsync(userId, updateUserDTO);
            if (result is null) return NotFound();
            return Ok(result);
        }
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await authService.DeleteUserAsync(userId);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
