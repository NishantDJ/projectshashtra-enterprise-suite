using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectShashtra.Constants;
using ProjectShashtra.Data;
using ProjectShashtra.Models;

namespace ProjectShashtra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
       
        public EmployeeController(
    IEmployeeRepository employeeRepository, ILogger<EmployeeController> logger, ApplicationDbContext context)
        {
            _context = context;            
            _logger = logger;
            _employeeRepository = employeeRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeRepository.GetAllAsync();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            var result = await _employeeRepository.AddAsync(employee);

            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(Employee employee)
        {
            bool result = await _employeeRepository.UpdateAsync(employee);

            if (!result)
                return NotFound();

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            bool result = await _employeeRepository.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
