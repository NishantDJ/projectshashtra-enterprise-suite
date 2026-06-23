using ProjectShashtra.Data;

namespace ProjectShashtra.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int id);

        Task<Employee> AddAsync(Employee employee);

        Task<bool> UpdateAsync(Employee employee);

        Task<bool> DeleteAsync(int id);

        Task<List<Employee>> GetEmployeesAsync(
            int pageNumber,
            int pageSize,
            string? department,
            string? sortBy);
    }
}
