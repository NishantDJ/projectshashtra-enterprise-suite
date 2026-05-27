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
        private readonly ILogger<EmployeeController> _logger;
        private readonly IProductRepository _repo;
        public EmployeeController(IProductRepository repo, ILogger<EmployeeController> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        [HttpGet]
        //[Authorize (Roles =Roles.Admin)]
        public IActionResult GetEmployees()
        {
            var result = _repo.GetEmployees();
            return Ok(result);
        }
    }
}
