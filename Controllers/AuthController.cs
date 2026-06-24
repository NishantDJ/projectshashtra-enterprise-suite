using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShashtra.DTOs;
using ProjectShashtra.Models;
using ProjectShashtra.Services.Interfaces;

namespace ProjectShashtra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            bool result =
                await _authService.RegisterAsync(dto);

            if (!result)
            {
                return Conflict(
                    ApiResponse<string>.Fail(
                        "Email already exists."));
            }

            return Ok(
                ApiResponse<string>.Ok(
                    "User registered successfully."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result =
                await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized(
                    ApiResponse<string>.Fail(
                        "Invalid username or password."));
            }

            return Ok(
                ApiResponse<AuthResponseDto>.Ok(
                    result,
                    "Login successful."));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            RefreshRequestDto dto)
        {
            var result =
                await _authService
                    .RefreshTokenAsync(dto.RefreshToken);

            if (result == null)
            {
                return Unauthorized(
                    ApiResponse<string>.Fail(
                        "Invalid refresh token."));
            }

            return Ok(
                ApiResponse<AuthResponseDto>.Ok(result));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            RefreshRequestDto dto)
        {
            await _authService
                .LogoutAsync(dto.RefreshToken);

            return Ok(
                ApiResponse<string>.Ok(
                    "Logged out successfully."));
        }
    }
}