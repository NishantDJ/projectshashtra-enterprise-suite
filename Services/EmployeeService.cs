using ProjectShashtra.Data;
using ProjectShashtra.Models;
using ProjectShashtra.Repositories;
using ProjectShashtra.Services.Interfaces;

namespace ProjectShashtra.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(
            IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {

            if (employee.Salary < 0)
                throw new Exception("Salary cannot be negative");

            return await _employeeRepository.AddAsync(employee);
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            if (employee.Salary < 0)
                throw new Exception("Salary cannot be negative");

            return await _employeeRepository.UpdateAsync(employee);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _employeeRepository.DeleteAsync(id);
        }

        public async Task<List<Employee>> GetEmployeesAsync(
            int pageNumber,
            int pageSize,
            string? department,
            string? sortBy)
        {
            return await _employeeRepository.GetEmployeesAsync(
                pageNumber,
                pageSize,
                department,
                sortBy);
        }
    }
}