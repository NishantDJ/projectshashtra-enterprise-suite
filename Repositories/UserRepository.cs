using ProjectShashtra.Repositories.Interfaces;

namespace ProjectShashtra.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBCS");
        }
    }
}
