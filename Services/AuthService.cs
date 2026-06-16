using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using ProjectShashtra.Data;
using ProjectShashtra.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjectShashtra.Services
{
    public class AuthService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _config;

        public AuthService(IConfiguration configuration)
        {
            _config = configuration;
            _connectionString = configuration.GetConnectionString("DBCS")!;
        }

        public async Task<bool> EmailExists(string email)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);

                await con.OpenAsync();

                int count = (int)(await cmd.ExecuteScalarAsync())!;
                return count > 0;
            }
        }

        public async Task<bool> RegisterUser(RegisterDTO dto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);

            string role = dto.Role is "Admin" or "User"
                ? dto.Role
                : "User";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_RegisterUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FullName", dto.Fullname);
                cmd.Parameters.AddWithValue("@Email", dto.Username);
                cmd.Parameters.AddWithValue("@Role", role);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                await conn.OpenAsync();

                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_GetUserByEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", email);

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserId = reader.GetInt32(0),
                        Fullname = reader.GetString(1),
                        Username = reader.GetString(2),
                        PasswordHash = reader.GetString(3),
                        Role = reader.GetString(4)
                    };
                }

                return null;
            }
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var credentials =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Username),
                new Claim("fullname", user.Fullname),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public async Task SaveRefreshTokenAsync(int userId, string token)
        {
            var expiryDays =
                Convert.ToInt32(_config["JwtSettings:RefreshTokenExpiryDays"]);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
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

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue(
                    "@ExpiresAt",
                    DateTime.UtcNow.AddDays(expiryDays));

                await con.OpenAsync();

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token)
        {
            string query = @"
                SELECT
                    Id,
                    UserId,
                    Token,
                    ExpiresAt,
                    IsRevoked
                FROM RefreshTokens
                WHERE Token = @Token
                AND IsRevoked = 0
                AND ExpiresAt > GETUTCDATE()";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Token", token);

                await con.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new RefreshToken
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        Token = reader.GetString(2),
                        ExpiresAt = reader.GetDateTime(3),
                        IsRevoked = reader.GetBoolean(4)
                    };
                }

                return null;
            }
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE RefreshTokens
                    SET IsRevoked = 1
                    WHERE Token = @Token";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Token", token);

                await con.OpenAsync();

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_GetUserById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", id);

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserId = reader.GetInt32(0),
                        Fullname = reader.GetString(1),
                        Username = reader.GetString(2),
                        PasswordHash = reader.GetString(3),
                        Role = reader.GetString(4)
                    };
                }

                return null;
            }
        }
    }
}