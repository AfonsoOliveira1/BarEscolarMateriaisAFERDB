using Microsoft.AspNetCore.Mvc;
using APiConsumer.Models;
using APiConsumer.Services;

namespace APiConsumer.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductsApiClient _productsApi;

        public ProductsController(ProductsApiClient productsApi)
        {
            _productsApi = productsApi;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productsApi.GetProductsAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PRODUCTS product)
        {
            if (!ModelState.IsValid)
                return View(product);

            await _productsApi.CreateProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productsApi.GetProductAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PRODUCTS product)
        {
            if (!ModelState.IsValid)
                return View(product);

            await _productsApi.UpdateProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _productsApi.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
