using Microsoft.EntityFrameworkCore;
using ProjectShashtra.Models;

namespace ProjectShashtra.Data
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeId == id);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            var existingEmployee =
                await _context.Employees
                    .FirstOrDefaultAsync(x => x.EmployeeId == employee.EmployeeId);

            if (existingEmployee == null)
                return false;

            existingEmployee.Department = employee.Department;
            existingEmployee.Designation = employee.Designation;
            existingEmployee.Salary = employee.Salary;
            existingEmployee.JoiningDate = employee.JoiningDate;
            existingEmployee.IsActive = employee.IsActive;
            existingEmployee.UserId = employee.UserId;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return false;

            _context.Employees.Remove(employee);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Employee>> GetEmployeesAsync(
            int pageNumber,
            int pageSize,
            string? department,
            string? sortBy)
        {
            IQueryable<Employee> query =
                _context.Employees.AsNoTracking();

            // Filtering

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(x =>
                    x.Department == department);
            }

            // Sorting

            query = sortBy?.ToLower() switch
            {
                "salary" => query.OrderByDescending(x => x.Salary),

                "department" => query.OrderBy(x => x.Department),

                "joiningdate" => query.OrderByDescending(x => x.JoiningDate),

                _ => query.OrderBy(x => x.EmployeeId)
            };

            // Pagination

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}