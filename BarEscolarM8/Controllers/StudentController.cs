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
        public async Task<IActionResult> Materiais()
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

            var data = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<MaterialViewModel>>();

            return View(data);
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
            var result = await response.Content.ReadAsStringAsync();

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
        public async Task<IActionResult> BarEscolar()
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Product");

            return View(response);
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