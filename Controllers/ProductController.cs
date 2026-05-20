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
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _repo;
        public ProductController(IProductRepository repo, ILogger<ProductController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles =$"{Roles.Admin},{Roles.User}")]
        public IActionResult Get()
        {
            _logger.LogInformation("Accessed GetProduct at {Time}",DateTime.UtcNow);
           
                int x = 0;
                int y = 5 / x;
           
           
            return Ok(_repo.GetProducts());

        }
        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public IActionResult GetById(int id)
        {
            return Ok(_repo.GetProductsById(id));

        }
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult InsertProducts(Product product)
        {
            if (product == null)
                return BadRequest();
            int newid = _repo.InsertProducts(product);
            return Ok(newid);
        }

        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult UpdateProduct(Product product)
        {
            if (product == null)
                return BadRequest();
            bool result = _repo.UpdateProduct(product);
            if (!result)
                return NotFound("Product not found");
            return Ok("Product Updated successfully");
        }
        [HttpDelete]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult DeleteProduct(int id)
        {
            if (id == null)
                return BadRequest();
            bool result = _repo.DeleteProduct(id);
            if (!result)
                return NotFound("Product not deleted");
            return Ok("Product deleted successfully");
        }
    }
}
