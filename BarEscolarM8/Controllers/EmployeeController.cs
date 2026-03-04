using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Azure;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BarEscolarM8.Controllers
{
    [Authorize(Roles = "EMPLOYEE")]

    public class EmployeeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public EmployeeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        // ---------------- DASHBOARD ----------------
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var prodResponse = await client.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Product");
            var catResponse = await client.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Category");

            var prodxcat = new ProdxCat
            {
                Products = prodResponse,
                Categorys = catResponse
            };

            return View(prodxcat);
        }

        public async Task<IActionResult> CreateProduct()
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var catResponse = await client.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Category");
            ViewBag.Categories = catResponse;
            return View("CreateProduct");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/Product/", model);
            var catResponse = await client.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Category");
            ViewBag.Categories = catResponse;
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View("CreateProduct");
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<ProductCreateDto>($"api/Product/{id}");
            var catResponse = await client.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Category");
            ViewBag.Categories = catResponse;
            if (response == null)
            {
                ModelState.AddModelError("", "Erro ao encontar o produto");
                return RedirectToAction("Index");
            }
            return View("EditProduct", response);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, ProductCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"api/Product/update/{id}", model);
            var catResponse = await client.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Category");
            ViewBag.Categories = catResponse;
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View("EditProduct", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"api/Product/delete/{id}");

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<ProductDto>($"api/Product/{id}");

            return View(response);
        }

        // ---------------- CATEGORY CRUD ----------------

        public IActionResult CreateCategory() => View("CreateCategory");

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/Category/", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View("CreateCategory");
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<CategoryCreateDto>($"api/Category/{id}");
            if (response == null)
            {
                ModelState.AddModelError("", "Erro ao encontar o produto");
                return RedirectToAction("Index");
            }
            return View("EditCategory", response);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, CategoryCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"api/Category/update/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View("EditCategory", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"api/Category/delete/{id}");

            return RedirectToAction("Index");
        }
    }
}
