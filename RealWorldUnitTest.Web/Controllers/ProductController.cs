using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest.Web.DAL;
using RealWorldUnitTest.Web.Models;

namespace RealWorldUnitTest.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(x => x.Category).ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();
            var productWithCategory = await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (productWithCategory is null) return NotFound();
            return View(productWithCategory);
        }
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            if (!ModelState.IsValid) return View(product);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var productsWithCategory = await _context.Products.Include(c => c.Category).FirstAsync(x => x.Id == id);

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productsWithCategory.CategoryId);

            if (productsWithCategory is null) return NotFound();

            return View(productsWithCategory);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, Product product)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            if (id != product.Id) return NotFound();

            if (!ModelState.IsValid) return View(product);

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var productsWithCategory = await _context.Products.Include(c => c.Category).FirstAsync(x => x.Id == id);

            if (productsWithCategory is null) return NotFound();

            return View(productsWithCategory);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id is null) return NotFound();

            var productsWithCategory = await _context.Products.Include(c => c.Category).FirstAsync(x => x.Id == id);

            if (productsWithCategory is null) return NotFound();

            _context.Products.Remove(productsWithCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}