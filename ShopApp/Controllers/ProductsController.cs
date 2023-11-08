using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models;

namespace ShopApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopAppDbContext _context;

        public ProductsController(ShopAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = _context.Products
                .Include(product => product.Category)
                .ToListAsync();
            
            return View(await products);
        }

        // GET: Products/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var selectedProduct = _context.Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            
            return View(await selectedProduct);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Category"] = new SelectList(_context.Set<Category>(), "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine(product.ProductName);
                Console.WriteLine(product.Price);

                // Masalahnya ada di categoryId
                Console.WriteLine(product.CategoryId);
                // Console.WriteLine(product.Category.Id);
                
                Console.WriteLine("Model state invalid");
                return RedirectToAction(nameof(Create));
            }


            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            var selectedProduct = _context.Products.Where(p => p.Id == id).FirstOrDefault();

            // Third param (selectedProduct?.Category?.CategoryName) is used to specify the default selected value.
            ViewData["Category"] = new SelectList(_context.Set<Category>(), "Id", "CategoryName", selectedProduct?.Category?.CategoryName);
            return View(selectedProduct);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state invalid");
                return RedirectToAction(nameof(Edit));
            }

            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public IActionResult Delete(int id)
        {
            var selectedProduct = _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .FirstOrDefault();

            return View(selectedProduct);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            Console.WriteLine(id);

            var selectedProduct = _context.Products
                .Where(p => p.Id == id)
                .FirstOrDefault();

            var relevantSalesDetails = _context.SalesDetails.Where(sd => sd.ProductId == selectedProduct.Id).ToList();

            foreach (var salesDetail in relevantSalesDetails)
            {
                _context.SalesDetails.Remove(salesDetail);
            }

            _context.Products.Remove(selectedProduct);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
