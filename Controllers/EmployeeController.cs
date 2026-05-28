using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IProductRepository _repo;
        public EmployeeController(IProductRepository repo, ILogger<EmployeeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _repo = repo;
            _logger = logger;
        }
        [HttpGet]
        //[Authorize (Roles =Roles.Admin)]
        public IActionResult GetEmployees()
        {
            var result = _context.Employees.ToList();
            return Ok(result);
        }
    }
}
