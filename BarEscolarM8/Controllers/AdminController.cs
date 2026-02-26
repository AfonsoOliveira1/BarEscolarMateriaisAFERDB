using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Azure;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarEscolarM8.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AdminController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // ---------------- DASHBOARD ----------------
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var weeksResponse = await client.GetFromJsonAsync<IEnumerable<MenuWeekDto>>("api/MenuWeek");
            var usersResponse = await client.GetFromJsonAsync<IEnumerable<UserReadDto>>("api/User");

            var userxweek = new WeekXUsers();
            userxweek.MenuWeek = weeksResponse;
            userxweek.Users = usersResponse;
            return View(userxweek);
        }
        public IActionResult CreateUser() => View("Users/CreateUser");

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/User/register", model);

            return RedirectToAction("Users");
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<UserUpdateDto>($"api/User/{id}");
            if (response == null)
            {
                ModelState.AddModelError("", "Erro ao encontar o user");
                return RedirectToAction("Index");
            }
            return View("EditUser", response); 
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserUpdateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"api/User/update/{model.Id}", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User updated successfully!";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Erro editar");
            return RedirectToAction("EditUser", model.Id);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"api/User/delete/{id}");

            return RedirectToAction("Index");
        }
        // ---------------- MENU WEEKS ----------------
        public IActionResult CreateWeek() => View("Create");

        [HttpPost]
        public async Task<IActionResult> CreateWeek(MenuWeekCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/MenuWeek", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao encontar o user");
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Semana criada com sucesso!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditWeek(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<MenuWeekCreateDto>($"api/MenuWeek/{id}");
            if (response == null)
            {
                ModelState.AddModelError("", "Erro ao encontar o menuweek");
                return RedirectToAction("Index");
            }
            return RedirectToAction("EditMenuWeek", response);
        }

        [HttpPost]
        public async Task<IActionResult> EditWeek(int id, MenuWeekCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"api/MenuWeek/update/{model.Id}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao editar o menuweek");
                return RedirectToAction("Index");
            }
            return View("EditMenuWeek", response);
        }
        public async Task<IActionResult> EditMenuDay(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/MenuDay/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao encontar o menuweek");
                return RedirectToAction("Index");
            }
            return View("EditMenuDay", response);
        }

        [HttpPost]
        public async Task<IActionResult> EditMenuDay(int id, MenuDayUpdateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"api/MenuWeek/update/{id}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao editar o menuweek");
                return RedirectToAction("Index");
            }
            return View("EditMenuWeek", response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWeek(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"api/MenuWeek/delete/{id}");

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteDay(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"api/MenuDay/delete/{id}");

            return RedirectToAction("Index");
        }
    }
}
