using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models;
using System.Security.Cryptography.Xml;

namespace ShopApp.Controllers
{
    public class SalesController : Controller
    {
        private readonly ShopAppDbContext _context;

        public SalesController(ShopAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Include is used to add the related tables.
            // I read somewhere that because of LazyLoad, we need to explicitly specify which data need to be included in the result.
            // If I remove the Include, the view page will produce a NullReference error.

            var sales = _context.Sales
                .Include(s => s.SalesDetails)
                .ToList();


            var salesDictionary = new Dictionary<int, List<SalesDetail>>();
            
            foreach (var sale in sales)
            {
                var salesDetailList = sale.SalesDetails.ToList();
                salesDictionary[sale.Id] = salesDetailList;
            }

            var products = _context.Products.ToList();
            var productDictionary = new Dictionary<int, Product>();

            foreach (var product in products)
            {
                productDictionary[product.Id] = product;
            }

            ViewBag.SalesDictionary = salesDictionary;
            ViewBag.ProductDictionary = productDictionary;

            return View(sales);
        }

        public IActionResult Detail(int id, int? productId)
        {
            var SalesDetail = _context.SalesDetails
                .Where(sd => sd.SalesId == id)
                .Include(sd => sd.Products)
                .ToList();

            if (SalesDetail.Count != 0)
            {
                ViewData["SalesDetailID"] = SalesDetail[0].SalesId;
            }
            else
            {
                ViewData["SalesDetailID"] = id;
            }

            Console.WriteLine(productId);

            if (productId != null)
            {
                var selectedProduct = _context.Products.Where(sp => sp.Id == productId).FirstOrDefault();
                ViewData["Products"] = new SelectList(_context.Set<Product>(), "Id", "ProductName", selectedProduct?.Id);
                
                var selectedSalesDetail = _context.SalesDetails.Where(sd => sd.SalesId == id && sd.ProductId == productId).FirstOrDefault();
                ViewData["SelectedProductQuantity"] = selectedSalesDetail?.Quantity;
            }
            else
            {
                ViewData["Products"] = new SelectList(_context.Set<Product>(), "Id", "ProductName");
                ViewData["SelectedProductQuantity"] = 1;
            }

            return View(SalesDetail);
        }

        [HttpPost]
        public IActionResult Create(Sales sales)
        {
            sales.SalesDate = DateTime.Now;

            _context.Add(sales);
            _context.SaveChanges();

            // Order the sales by Id, reverse it, then take the first item (the latest sales data created).
            var lastSales = _context.Sales.OrderBy(s => s.Id).Reverse().FirstOrDefault();
            Console.WriteLine(lastSales?.Id);

            // return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Detail), new { id = lastSales?.Id });
        }


        public IActionResult Delete(int id)
        {
            // Delete the sales details first, and then the sales data.

            var relevantSalesDetails = _context.SalesDetails.Where(sd => sd.SalesId == id).ToList();

            foreach (var item in relevantSalesDetails)
            {
                _context.SalesDetails.Remove(item);
            }

            var relevantSales = _context.Sales.Where(s => s.Id == id).FirstOrDefault();
            
            _context.Sales.Remove(relevantSales); 
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult CreateSalesDetail([Bind("SalesId,ProductId,Quantity")] SalesDetail salesDetail)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state invalid");
                return RedirectToAction(nameof(Detail), new { id = salesDetail.SalesId });
            }

            var salesDetailValidation = _context.SalesDetails
                .Where(sd => sd.SalesId == salesDetail.SalesId && sd.ProductId == salesDetail.ProductId)
                .FirstOrDefault();

            Console.WriteLine(salesDetailValidation);

            if (salesDetailValidation != null)
            {
                salesDetailValidation.Quantity = salesDetail.Quantity;
                _context.SalesDetails.Update(salesDetailValidation);
                _context.SaveChanges();
            }
            else
            {
                _context.SalesDetails.Add(salesDetail);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Detail), new { id = salesDetail.SalesId });
        }

        public IActionResult EditSalesDetail(int id, int productId)
        {
            return RedirectToAction(nameof(Detail), new { id = id , productId = productId});
        }

        [HttpPost]
        public IActionResult DeleteSalesDetail(int id, int productId)
        {
            var selectedDeleteSalesDetail = _context.SalesDetails.Where(sd => sd.SalesId == id && sd.ProductId == productId).FirstOrDefault();

            _context.SalesDetails.Remove(selectedDeleteSalesDetail);
            _context.SaveChanges();

            return RedirectToAction(nameof(Detail), new { id = id });
        }
    }
}
