using ProjectShashtra.DTOs;
using ProjectShashtra.Models;

namespace ProjectShashtra.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDTO dto);

        Task<AuthResponseDto?> LoginAsync(LoginDto dto);

        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);

        Task<bool> LogoutAsync(string refreshToken);
    }
}