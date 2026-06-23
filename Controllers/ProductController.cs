using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectShashtra.Constants;
using ProjectShashtra.Models;
using ProjectShashtra.Repositories.Interfaces;
using ProjectShashtra.Services.Interfaces;
using System.Linq;

namespace ProjectShashtra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _service;

        public ProductController(IProductService service,ILogger<ProductController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles =$"{Roles.Admin},{Roles.User}")]
        public IActionResult Get()
        {
            _logger.LogInformation("Accessed GetProduct at {Time}",DateTime.UtcNow);
           
                //int x = 0;
                //int y = 5 / x;
           
           
            var product = _service.GetProducts();

            //  LINQ 
            var result1 = product
                .Where(p => p.Price > 500)
                .OrderBy(p => p.Name)
                .ToList();

            //  LINQ 
            var result = result1 
                .Select(r=> new{
                r.Name,
                r.Price})
                .ToList();
            return Ok(result);

        }
        [HttpGet("{id}")]
        //[Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
        public IActionResult GetById(int id)
        {
            return Ok(_service.GetProductsById(id));

        }
        [HttpPost]
        //[Authorize(Roles = Roles.Admin)]
        public IActionResult InsertProducts(Product product)
        {
            if (product == null)
                return BadRequest();
            int newid = _service.InsertProducts(product);
            return Ok(newid);
        }

        [HttpPut]
        //[Authorize(Roles = Roles.Admin)]
        public IActionResult UpdateProduct(Product product)
        {
            if (product == null)
                return BadRequest();
            bool result = _service.UpdateProduct(product);
            if (!result)
                return NotFound("Product not found");
            return Ok("Product Updated successfully");
        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = Roles.Admin)]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
                return BadRequest();
            bool result = _service.DeleteProduct(id);
            if (!result)
                return NotFound("Product not deleted");
            return Ok("Product deleted successfully");
        }
    }
}
