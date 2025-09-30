using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest.Web.DAL;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repositories;

namespace RealWorldUnitTest.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _repository;
        public ProductController(IRepository<Product> repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            var product = await _repository.GetByIdAsync((int)id);

            if (product is null) return NotFound();

            return View(product);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) return View(product);

            await _repository.Create(product);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            var product = await _repository.GetByIdAsync((int)id);

            if (product is null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, int id)
        {
            if (product.Id != id) return BadRequest();

            if (!ModelState.IsValid) return View(product);

            _repository.Update(product);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var product = await _repository.GetByIdAsync((int)id);

            if (product is null) return NotFound();

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null) return NotFound();
            _repository.Delete(product);
            return RedirectToAction(nameof(Index));
        }
        
    }
}
