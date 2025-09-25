using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWorldUnitTest.Web.Helpers;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;

namespace RealWorldUnitTest.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IRepository<Product> _repository;
        public ProductApiController(IRepository<Product> repository)
        {
            _repository = repository;
        }
        [HttpGet("{a}/{b}")]
        public IActionResult Add(int a, int b)
        {
            return Ok(new Helper().Add(a, b));
        }
        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null) return NotFound();

            return Ok(product);
        }
        [HttpPut("{id}")]
        public IActionResult PutProduct(int? id, Product product)
        {
            if (id != product.Id) return BadRequest();

            _repository.Update(product);

            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _repository.Create(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null) return NotFound();

            _repository.Delete(product);

            return NoContent();
        }
        public bool CheckProductExists(int id)
        {
            var product = _repository.GetByIdAsync(id).Result;

            if (product is null) return false;
            else return true;
        }

    }
}
