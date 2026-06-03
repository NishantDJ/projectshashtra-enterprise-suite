using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectShashtra.Data;
using ProjectShashtra.Models;
using ProjectShashtra.Services;

namespace ProjectShashtra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDTO dto)
        {

            _logger.LogInformation("User register at {Time}", DateTime.UtcNow);

            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Fullname) ||
                string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                return BadRequest(new { message = "All feilds are required." });
            }
            bool emailExixts = await _authService.EmailExists(dto.Username);
            if (emailExixts)
                return BadRequest(new { message = "Email already registered" });
            bool success = await _authService.RegisterUser(dto);
            if (!success) 
                return StatusCode(500,new {message  = "Registeration failed"});
            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "Username and Password are required." });
            }
            var user = await _authService.GetUserByEmailAsync(dto.Username);
            if (user == null)
                return BadRequest(new { message = "Invalid Username or Password" });
            string token = _authService.GenerateJwtToken(user);
            string refreshToken = _authService.GenerateRefreshToken();
            await _authService.SaveRefreshTokenAsync(user.UserId, refreshToken);

            return Ok(new
            {
                message = "Login successful.",
                token = token,
                refreshToken=refreshToken,
                fullName = user.Fullname,
                username = user.Username,
                role=user.Role

            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken)) 
                return BadRequest(new { message = "Invalid or Expired token." });

            // Retrieve the refresh token record
            var refreshToken = await _authService.GetValidRefreshTokenAsync(dto.RefreshToken);
            if (refreshToken == null)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            // Get the user associated with the refresh token
            var user = await _authService.GetUserByIdAsync(refreshToken.UserId);
            if (user == null)
                return Unauthorized(new { message = "User not found." });

            // Revoke the old refresh token and issue new tokens
            await _authService.RevokeRefreshTokenAsync(dto.RefreshToken);

            string newAccessToken = _authService.GenerateJwtToken(user);
            string newRefreshToken = _authService.GenerateRefreshToken();

            // Use the same user id property used elsewhere (e.g. user.UserId)
            await _authService.SaveRefreshTokenAsync(user.UserId, newRefreshToken);

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });


        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody]RefreshToken dto)
        {
            if(string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest(new { message = "Refresh token id reqiuired." } );
            await _authService.RevokeRefreshTokenAsync(dto.Token);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
