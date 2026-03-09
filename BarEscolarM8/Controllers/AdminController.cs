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
            var prodResponse = await client.GetFromJsonAsync<IEnumerable<ProductDto>>("api/Product");
            var catResponse = await client.GetFromJsonAsync<IEnumerable<CategoryDto>>("api/Category");
            var matResponse = await client.GetFromJsonAsync<IEnumerable<MaterialViewModel>>("api/Material");
            var matcatResponse = await client.GetFromJsonAsync<IEnumerable<MaterialCategoryViewDto>>("api/MaterialCategory");
            var daysResponse = await client.GetFromJsonAsync<IEnumerable<MenuDayDto>>("api/MenuDay");

            var userxweek = new WeekXUsers();
            userxweek.MenuWeek = weeksResponse;
            userxweek.Users = usersResponse;

            userxweek.Counts = new Counts();
            userxweek.Counts.weeksCount = weeksResponse?.Count() ?? 0;
            userxweek.Counts.usersCount = usersResponse?.Count() ?? 0;
            userxweek.Counts.daysCount = daysResponse?.Count() ?? 0;
            userxweek.Counts.matCount = matResponse?.Count() ?? 0;
            userxweek.Counts.matcatCount = matcatResponse?.Count() ?? 0;
            userxweek.Counts.prodCount = prodResponse?.Count() ?? 0;
            userxweek.Counts.catCount = catResponse?.Count() ?? 0;

            return View(userxweek);
        }
        public IActionResult CreateUser() => View("CreateUser");

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/User/register", model);
            if(response.IsSuccessStatusCode)
            {
                TempData["Success"] = "User created successfully!";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Erro ao criar o user");
            return View("CreateUser");
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
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View("EditUser", model);
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
        public IActionResult CreateMenuWeek() => View("CreateMenuWeek");

        [HttpPost]
        public async Task<IActionResult> CreateMenuWeek(MenuWeekCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/MenuWeek", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View("CreateMenuWeek", model);
            }

            TempData["Success"] = "Semana criada com sucesso!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditMenuWeek(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<MenuWeekDto>($"api/MenuWeek/{id}");
            if (response == null)
            {
                ModelState.AddModelError("", "Erro ao encontar o menuweek");
                return RedirectToAction("Index");
            }
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> EditMenuWeek(int id, MenuWeekCreateDto model)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"api/MenuWeek/update/{model.Id}", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                var reload = await client.GetFromJsonAsync<MenuWeekDto>($"api/MenuWeek/{id}");
                return View("EditMenuWeek", reload);
            }

            return RedirectToAction("EditMenuWeek", new { id = model.Id });

        }

        public async Task<IActionResult> EditMenuDay(int id)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetFromJsonAsync<MenuDayUpdateDto>($"api/MenuDay/{id}");

            if (response == null)
            {
                ModelState.AddModelError("", "Erro ao encontar o menuday");
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
            var response = await client.PutAsJsonAsync($"api/MenuDay/update/{id}", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View("EditMenuDay", model);
            }

            return RedirectToAction("EditMenuWeek", new { id = model.Menuweekid });
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
        public async Task<IActionResult> DeleteDay(int id, int menuweekid)
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"api/MenuDay/delete/{id}");

            return RedirectToAction("EditMenuWeek", new { id = menuweekid });
        }
    }
}
