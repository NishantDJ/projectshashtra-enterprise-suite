using ProjectShashtra.Models;

namespace ProjectShashtra.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);

        Task<int> RegisterUserAsync(
            string fullName,
            string email,
            string passwordHash,
            string role);

        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserByIdAsync(int id);

        Task SaveRefreshTokenAsync(
            int userId,
            string token,
            DateTime expiresAt);

        Task<RefreshToken?> GetValidRefreshTokenAsync(
            string token);

        Task RevokeRefreshTokenAsync(string token);
    }
}