using Microsoft.Data.SqlClient;
using ProjectShashtra.Models;
using ProjectShashtra.Repositories.Interfaces;
using System.Data;

namespace ProjectShashtra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DBCS")!;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using SqlConnection con =
                new(_connectionString);

            string query =
                "SELECT COUNT(1) FROM Users WHERE Email=@Email";

            SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@Email", email);

            await con.OpenAsync();

            int count =
                (int)(await cmd.ExecuteScalarAsync())!;

            return count > 0;
        }

        public async Task<int> RegisterUserAsync(
            string fullName,
            string email,
            string passwordHash,
            string role)
        {
            using SqlConnection conn =
                new(_connectionString);

            SqlCommand cmd =
                new("usp_RegisterUser", conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FullName", fullName);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@Role", role);

            await conn.OpenAsync();

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using SqlConnection conn =
                new(_connectionString);

            SqlCommand cmd =
                new("usp_GetUserByEmail", conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserName", email);

            await conn.OpenAsync();

            using var reader =
                await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new User
            {
                UserId = reader.GetInt32(0),
                Fullname = reader.GetString(1),
                Username = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                Role = reader.GetString(4)
            };
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using SqlConnection conn =
                new(_connectionString);

            SqlCommand cmd =
                new("usp_GetUserById", conn);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", id);

            await conn.OpenAsync();

            using var reader =
                await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new User
            {
                UserId = reader.GetInt32(0),
                Fullname = reader.GetString(1),
                Username = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                Role = reader.GetString(4)
            };
        }

        public async Task SaveRefreshTokenAsync(
            int userId,
            string token,
            DateTime expiresAt)
        {
            using SqlConnection con =
                new(_connectionString);

            string query = @"
                INSERT INTO RefreshTokens
                (
                    UserId,
                    Token,
                    ExpiresAt
                )
                VALUES
                (
                    @UserId,
                    @Token,
                    @ExpiresAt
                )";

            SqlCommand cmd =
                new(query, con);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.Parameters.AddWithValue("@ExpiresAt", expiresAt);

            await con.OpenAsync();

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<RefreshToken?> GetValidRefreshTokenAsync(
            string token)
        {
            using SqlConnection con =
                new(_connectionString);

            string query = @"
                SELECT
                    Id,
                    UserId,
                    Token,
                    ExpiresAt,
                    IsRevoked
                FROM RefreshTokens
                WHERE Token=@Token
                AND IsRevoked=0
                AND ExpiresAt > GETUTCDATE()";

            SqlCommand cmd =
                new(query, con);

            cmd.Parameters.AddWithValue("@Token", token);

            await con.OpenAsync();

            using var reader =
                await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new RefreshToken
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Token = reader.GetString(2),
                ExpiresAt = reader.GetDateTime(3),
                IsRevoked = reader.GetBoolean(4)
            };
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            using SqlConnection con =
                new(_connectionString);

            string query =
                @"UPDATE RefreshTokens
                  SET IsRevoked=1
                  WHERE Token=@Token";

            SqlCommand cmd =
                new(query, con);

            cmd.Parameters.AddWithValue("@Token", token);

            await con.OpenAsync();

            await cmd.ExecuteNonQueryAsync();
        }
    }
}