using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShashtra.Constants;
using ProjectShashtra.Data;
using ProjectShashtra.Models;

namespace ProjectShashtra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IEmployeeRepository employeeRepository,
            ILogger<EmployeeController> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        // GET: api/Employee?pageNumber=1&pageSize=10&department=IT&sortBy=salary
        [HttpGet]
        //[Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetEmployees(
            int pageNumber = 1,
            int pageSize = 10,
            string? department = null,
            string? sortBy = null)
        {
            _logger.LogInformation("Fetching employees");

            var employees = await _employeeRepository.GetAllAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(department))
            {
                employees = employees
                    .Where(e => e.Department == department)
                    .ToList();
            }

            // Sorting
            employees = sortBy?.ToLower() switch
            {
                "salary" => employees.OrderByDescending(e => e.Salary).ToList(),
                "name" => employees.OrderBy(e => e.Department).ToList(),
                _ => employees.OrderBy(e => e.EmployeeId).ToList()
            };

            // Pagination
            var pagedData = employees
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                TotalRecords = employees.Count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = pagedData
            });
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        //[Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            _logger.LogInformation("Fetching employee with Id {Id}", id);

            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee not found. Id {Id}", id);
                return NotFound(new { Message = "Employee not found" });
            }

            return Ok(employee);
        }

        // POST: api/Employee
        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Creating employee");

            var result = await _employeeRepository.AddAsync(employee);

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = result.EmployeeId },
                result);
        }

        // PUT: api/Employee
        [HttpPut]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Updating employee Id {Id}", employee.EmployeeId);

            bool result = await _employeeRepository.UpdateAsync(employee);

            if (!result)
            {
                _logger.LogWarning("Employee not found for update. Id {Id}", employee.EmployeeId);
                return NotFound(new { Message = "Employee not found" });
            }

            return Ok(new { Message = "Employee updated successfully" });
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogWarning("Deleting employee Id {Id}", id);

            bool result = await _employeeRepository.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Employee not found for delete. Id {Id}", id);
                return NotFound(new { Message = "Employee not found" });
            }

            return Ok(new { Message = "Employee deleted successfully" });
        }
    }
}