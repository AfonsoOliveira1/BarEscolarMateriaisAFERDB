using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Azure;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BarEscolarM8.Controllers
{
    [Authorize(Roles = "STUDENT")]
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public StudentController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // ---------------- MENUs --------------------
        public async Task<IActionResult> Index(string option = "A")
        {
            if (option == "A")
                option = "Non-Vegan";
            else
                option = "Vegan";

            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<IEnumerable<MenuWeekDto>>($"api/MenuWeek/option/{option}");
            var usermenus = await client.GetFromJsonAsync<IEnumerable<MenuMarcadoDto>>($"/api/MenuDay/MenusMarked/User/{user}");
            if (usermenus.Any())
            {
                ViewBag.MarkedMenuIds = usermenus.Select(u => u.Id).ToList();
            }
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Marcar(int menuId, string option)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            MarkMenuDto mark = new MarkMenuDto { Id = User.FindFirst(ClaimTypes.NameIdentifier).Value, menuId = menuId };

            var httpResponse = await client.PostAsJsonAsync($"/api/MenuDay/MarkMenu", mark);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                TempData["ErroMarcar"] = error;
                return RedirectToAction("Index", new {option = option});
            }
            return RedirectToAction("Index", new { option = option });
        }

        public async Task<IActionResult> MenusMarcados()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var usermenus = await client.GetFromJsonAsync<IEnumerable<MenuMarcadoDto>>($"/api/MenuDay/MenusMarked/User/{user}");
            return View(usermenus);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var usermenus = await client.DeleteAsync($"/api/MenuDay/cancel/{orderId}");
            if (!usermenus.IsSuccessStatusCode)
            {
                var error = await usermenus.Content.ReadAsStringAsync();
                TempData["MenusMarcados"] = error;
                return RedirectToAction("MenusMarcados");
            }
            return RedirectToAction("MenusMarcados");
        }
        // ---------------- MATERIAIS ----------------
        public async Task<IActionResult> Materiais(int? categoryId, string? minPrice, string? maxPrice)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await client.GetAsync($"api/Material");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View(new List<MaterialViewModel>().AsEnumerable());
            }

            var allMaterials = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<MaterialViewModel>>();

            // Criar lista de categorias
            var categorias = allMaterials
                .Select(m => new { CategoryId = m.Categoryid, CategoryName = $"Categoria {m.Categoryid}" })
                .Distinct()
                .OrderBy(c => c.CategoryId)
                .ToList();

            ViewBag.Categorias = categorias;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            decimal? min = null;
            decimal? max = null;

            if (!string.IsNullOrWhiteSpace(minPrice) &&
                decimal.TryParse(minPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var minParsed))
                min = minParsed;

            if (!string.IsNullOrWhiteSpace(maxPrice) &&
                decimal.TryParse(maxPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var maxParsed))
                max = maxParsed;

            var materials = allMaterials;

            if (categoryId.HasValue)
                materials = materials.Where(m => m.Categoryid == categoryId.Value);

            if (min.HasValue)
                materials = materials.Where(m => m.Price >= min.Value);

            if (max.HasValue)
                materials = materials.Where(m => m.Price <= max.Value);

            return View(materials);
        }


        [HttpPost]
        public async Task<IActionResult> ComprarMaterial(int materialId, int quantidade)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync(
                "api/Material/Comprar",
                new ComprarMaterialDto { MaterialId = materialId, Quantidade = quantidade }
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErroMateriais"] = error;
                return RedirectToAction("Materiais");
            }

            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var usersResponse = await client.GetFromJsonAsync<UserReadDto>($"api/User/{userid}");
            var identity = (ClaimsIdentity)User.Identity;

            var oldClaim = identity.FindFirst("Saldo");
            if (oldClaim != null)
            {
                identity.RemoveClaim(oldClaim);
            }

            identity.AddClaim(new Claim("Saldo", usersResponse.Saldo.ToString()));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
            return RedirectToAction("Materiais");
        }

        public async Task<IActionResult> Historico(string id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await client.GetAsync($"api/Historico/User/{id}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View(new List<HistoricoViewDto>().AsEnumerable());
            }

            var data = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<HistoricoViewDto>>();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHistorico(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"api/Historico/delete/{id}");

            return RedirectToAction("Historico", new { id = User.FindFirst(ClaimTypes.NameIdentifier).Value });
        }
        // ---------------- PRODUTOS ---------------- 
        public async Task<IActionResult> BarEscolar(int? categoryId, string? minPrice, string? maxPrice)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var allProducts = await client.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Product");

            var categorias = allProducts
                .Where(p => p.CategoryId.HasValue)
                .Select(p => new { CategoryId = p.CategoryId.Value, p.CategoryName })
                .Distinct()
                .OrderBy(c => c.CategoryName)
                .ToList();

            ViewBag.Categorias = categorias;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            decimal? min = null;
            decimal? max = null;

            if (!string.IsNullOrWhiteSpace(minPrice) &&
                decimal.TryParse(minPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var minParsed))
            {
                min = minParsed;
            }

            if (!string.IsNullOrWhiteSpace(maxPrice) &&
                decimal.TryParse(maxPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var maxParsed))
            {
                max = maxParsed;
            }

            var products = allProducts;

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId.Value);

            if (min.HasValue)
                products = products.Where(p => p.Price >= min.Value);

            if (max.HasValue)
                products = products.Where(p => p.Price <= max.Value);

            return View(products);
        }

    public async Task<IActionResult> Details(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<ProductDto>($"api/Product/{id}");

            return View(response);
        }
        public async Task<IActionResult> ComprarProduto(int id, int qtd)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync($"api/Product/Comprar/", new ComprarProdutoDto { ProductId = id, Quantidade = qtd});
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErroCompra"] = error;
                return RedirectToAction("BarEscolar");
            }
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var usersResponse = await client.GetFromJsonAsync<UserReadDto>($"api/User/{userid}");
            var identity = (ClaimsIdentity)User.Identity;

            var oldClaim = identity.FindFirst("Saldo");
            if (oldClaim != null)
            {
                identity.RemoveClaim(oldClaim);
            }

            identity.AddClaim(new Claim("Saldo", usersResponse.Saldo.ToString()));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            return RedirectToAction("BarEscolar");
        }

        public async Task<IActionResult> ProdutosComprados()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var userprodutos = await client.GetFromJsonAsync<IEnumerable<ProductBoughtDto>>($"/api/Product/ProductsBought/User/{user}");
            if (!userprodutos.Any())
            {
                TempData["Erro"] = "Não Tem Produtos Comprados";
                return View(Enumerable.Empty<ProductBoughtDto>());
            }
            return View(userprodutos);
        }

        // ---------- SALDO REQUEST --------------
        public IActionResult SaldoUpdate() => View();

        [HttpPost]
        public async Task<IActionResult> SaldoUpdate(decimal saldo)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var saldorequest = await client.PostAsJsonAsync($"/api/User/saldo/request/{user}", saldo);
            if (!saldorequest.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Saldo Invalido");
                return View();
            }
            return RedirectToAction("Index");
        }
    }
}