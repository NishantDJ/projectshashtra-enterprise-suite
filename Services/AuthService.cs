using Microsoft.IdentityModel.Tokens;
using ProjectShashtra.DTOs;
using ProjectShashtra.Models;
using ProjectShashtra.Repositories.Interfaces;
using ProjectShashtra.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjectShashtra.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _config;

        public AuthService(
            IUserRepository repository,
            IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            if (await _repository.EmailExistsAsync(dto.Username))
                return false;

            string hash =
                BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);

            string role =
                dto.Role == "Admin"
                ? "Admin"
                : "User";

            int rows =
                await _repository.RegisterUserAsync(
                    dto.Fullname,
                    dto.Username,
                    hash,
                    role);

            return rows > 0;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user =
                await _repository.GetUserByEmailAsync(
                    dto.Username);

            if (user == null)
                return null;

            bool valid =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash);

            if (!valid)
                return null;

            string accessToken =
                GenerateJwtToken(user);

            string refreshToken =
                GenerateRefreshToken();

            await SaveRefreshToken(user.UserId, refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.Fullname,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(
            string refreshToken)
        {
            var savedToken =
                await _repository
                    .GetValidRefreshTokenAsync(refreshToken);

            if (savedToken == null)
                return null;

            var user =
                await _repository
                    .GetUserByIdAsync(savedToken.UserId);

            if (user == null)
                return null;

            await _repository
                .RevokeRefreshTokenAsync(refreshToken);

            string accessToken =
                GenerateJwtToken(user);

            string newRefreshToken =
                GenerateRefreshToken();

            await SaveRefreshToken(
                user.UserId,
                newRefreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                FullName = user.Fullname,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            await _repository
                .RevokeRefreshTokenAsync(refreshToken);

            return true;
        }

        private async Task SaveRefreshToken(
            int userId,
            string token)
        {
            int expiryDays =
                Convert.ToInt32(
                    _config["JwtSettings:RefreshTokenExpiryDays"]);

            await _repository.SaveRefreshTokenAsync(
                userId,
                token,
                DateTime.UtcNow.AddDays(expiryDays));
        }

        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];

            using var rng =
                RandomNumberGenerator.Create();

            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        private string GenerateJwtToken(User user)
        {
            var jwt =
                _config.GetSection("JwtSettings");

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwt["Key"]!));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.UserId.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role),

                new Claim(
                    "fullname",
                    user.Fullname)
            };

            var token =
                new JwtSecurityToken(
                    issuer: jwt["Issuer"],
                    audience: jwt["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        Convert.ToDouble(
                            jwt["ExpiryInMinutes"])),
                    signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}