using ProjectShashtra.Data;

namespace ProjectShashtra.Models
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "user";
        public DateTime CreatedAt {  get; set; }

        public Employee? Employee { get; set; }
    }
}
