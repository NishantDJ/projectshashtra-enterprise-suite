using Microsoft.EntityFrameworkCore;
using ProjectShashtra.Models;

namespace ProjectShashtra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }
        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }

}