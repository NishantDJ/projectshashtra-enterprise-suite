
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShashtra.DTOs;
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
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            _logger.LogInformation(
                "Register attempt for {Email} at {Time}",
                dto.Username,
                DateTime.UtcNow
            );

            if (await _authService.EmailExists(dto.Username))
            {
                return Conflict(
                    ApiResponse<string>.Fail("Email already registered.")
                );
            }

            bool success = await _authService.RegisterUser(dto);

            if (!success)
            {
                return StatusCode(
                    500,
                    ApiResponse<string>.Fail("Registration failed.")
                );
            }

            return Ok(
                ApiResponse<string>.Ok("User registered successfully.")
            );
        }

         
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.GetUserByEmailAsync(dto.Username);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(
                    ApiResponse<string>.Fail("Invalid username or password.")
                );
            }

            string accessToken = _authService.GenerateJwtToken(user);
            string refreshToken = _authService.GenerateRefreshToken();

            await _authService.SaveRefreshTokenAsync(user.UserId, refreshToken);

            _logger.LogInformation(
                "User {Email} logged in at {Time}",
                user.Username,
                DateTime.UtcNow
            );

            return Ok(
                ApiResponse<object>.Ok(
                    new
                    {
                        accessToken,
                        refreshToken,
                        fullName = user.Fullname,
                        username = user.Username,
                        role = user.Role
                    },
                    "Login successful."
                )
            );
        }

         
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var savedToken = await _authService.GetValidRefreshTokenAsync(dto.RefreshToken);

            if (savedToken == null)
            {
                return Unauthorized(
                    ApiResponse<string>.Fail("Invalid or expired refresh token.")
                );
            }

            var user = await _authService.GetUserByIdAsync(savedToken.UserId);

            if (user == null)
            {
                return Unauthorized(
                    ApiResponse<string>.Fail("User not found.")
                );
            }

            await _authService.RevokeRefreshTokenAsync(dto.RefreshToken);

            string newAccessToken = _authService.GenerateJwtToken(user);
            string newRefreshToken = _authService.GenerateRefreshToken();

            await _authService.SaveRefreshTokenAsync(user.UserId, newRefreshToken);

            return Ok(
                ApiResponse<object>.Ok(
                    new
                    {
                        accessToken = newAccessToken,
                        refreshToken = newRefreshToken
                    }
                )
            );
        }

       
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            await _authService.RevokeRefreshTokenAsync(dto.RefreshToken);

            return Ok(
                ApiResponse<string>.Ok("Logged out successfully.")
            );
        }
    }
}

