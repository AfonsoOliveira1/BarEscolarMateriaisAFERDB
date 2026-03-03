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
        public async Task<IActionResult> Index(string option = "A")
        {
            if (option == "A")
                option = "Non-Vegan";
            else
                option = "Vegan";

            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<IEnumerable<MenuWeekDto>>($"api/MenuWeek/option/{option}");

            return View(response);
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
    }
}