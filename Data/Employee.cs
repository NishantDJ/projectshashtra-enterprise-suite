using ProjectShashtra.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectShashtra.Data
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public int UserId { get; set; }

        //public string FullName { get; set; }

        //public string Email { get; set; }

        //public string Role { get; set; }

        public string Department { get; set; }

        public string Designation { get; set; }

        public decimal Salary { get; set; }

        public DateTime JoiningDate { get; set; }


        public bool IsActive { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool IsActive { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
    }
}