using APiConsumer.Models;
using APiConsumer.Services;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace APiConsumer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MenuWeeksApiClient _menuWeeksApi;
        private readonly MenuDaysApiClient _menuDaysApi;
        private readonly ProductsApiClient _productsApi;
        private readonly CategoryApiClient _categoriesApi;
        private readonly UsersApiClient _usersApi;
        private readonly MaterialsApiClient _materialsApi;
        private readonly MaterialCategoriesApiClient _materialCategoriesApi;
        public AdminController(
            MenuWeeksApiClient menuWeeksApi,
            MenuDaysApiClient menuDaysApi,
            ProductsApiClient productsApi,
            CategoryApiClient categoriesApi,
            UsersApiClient usersApi,
            MaterialsApiClient materialsApi,
            MaterialCategoriesApiClient materialCategoriesApi)
        {
            _menuWeeksApi = menuWeeksApi;
            _menuDaysApi = menuDaysApi;
            _productsApi = productsApi;
            _categoriesApi = categoriesApi;
            _usersApi = usersApi;
            _materialsApi = materialsApi;
            _materialCategoriesApi = materialCategoriesApi;
        }

        // ---------------- DASHBOARD ----------------
        public async Task<IActionResult> Index()
        {
            ViewBag.Weeks = await _menuWeeksApi.GetMenuWeeksAsync();
            ViewBag.Products = await _productsApi.GetProductsAsync();
            ViewBag.Categories = await _categoriesApi.GetCategoriesAsync();
            ViewBag.Materials = await _materialsApi.GetMaterialsAsync();
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _usersApi.GetUsersAsync();
            return View("Users/IndexUsers", users); 
        }

        public IActionResult CreateUser()
        {
            return View("Users/CreateUser"); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(
            string fullName, string email, string username, string password, int role)
        {
            var hasher = new PasswordHasher<USERS>();

            var newUser = new USERS
            {
                id = Guid.NewGuid().ToString(),
                fullname = fullName,
                email = email,
                username = username,
                role = role
            };

            newUser.passwordhash = hasher.HashPassword(newUser, password);

            var (success, error) = await _usersApi.CreateUserAsync(newUser);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "Failed to create user");
                return View("Users/CreateUser");
            }

            TempData["Success"] = "User created successfully!";
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _usersApi.GetUserAsync(id);
            if (user == null) return NotFound();

            return View("Users/EditUser", user); 
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(
            string id, string fullName, string email, string username, string? password, int role)
        {
            var user = await _usersApi.GetUserAsync(id);
            if (user == null) return NotFound();

            user.fullname = fullName;
            user.email = email;
            user.username = username;
            user.role = role;

            if (!string.IsNullOrWhiteSpace(password))
            {
                var hasher = new PasswordHasher<USERS>();
                user.passwordhash = hasher.HashPassword(user, password);
            }

            var success = await _usersApi.UpdateUserAsync(user);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to update user");
                return View("Users/EditUser", user);
            }

            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var success = await _usersApi.DeleteUserAsync(id);

            if (!success)
            {
                TempData["Error"] = "Failed to delete user.";
            }
            else
            {
                TempData["Success"] = "User deleted successfully!";
            }

            return RedirectToAction("Users");
        }
        // ---------------- MENU WEEKS ----------------
        public async Task<IActionResult> MenuWeeks()
        {
            var weeks = await _menuWeeksApi.GetMenuWeeksAsync();
            return View("MenuWeeks/Index", weeks);
        }

        public IActionResult CreateWeek()
        {
            return View("MenuWeeks/Create", new CreateWeekViewModel { WeekStart = DateTime.Today });
        }

        [HttpPost]
        public async Task<IActionResult> CreateWeek(DateTime weekStart)
        {
            if (weekStart.DayOfWeek != DayOfWeek.Monday)
            {
                ModelState.AddModelError("weekStart", "A semana tem que começar numa Segunda como é lógico.");
                ViewBag.EnteredDate = weekStart.ToString("yyyy-MM-dd");
                return View("MenuWeeks/Create");
            }

            var week = new MENUWEEK
            {
                weekstart = weekStart.ToString("yyyy-MM-dd")
            };

            var createdWeek = await _menuWeeksApi.CreateMenuWeekAsync(week);

            if (createdWeek == null)
            {
                ModelState.AddModelError("", "Falha ao criar a semana.");
                ViewBag.EnteredDate = weekStart.ToString("yyyy-MM-dd");
                return View("MenuWeeks/Create");
            }

            // Create menu days
            for (int i = 0; i < 5; i++)
            {
                var dayDate = DateOnly.FromDateTime(weekStart.AddDays(i));

                var veganDay = new MENUDAY
                {
                    menuweekid = createdWeek.Id,
                    date = dayDate,
                    option = "Vegan"
                };
                await _menuDaysApi.CreateMenuDayAsync(veganDay);

                var nonVeganDay = new MENUDAY
                {
                    menuweekid = createdWeek.Id,
                    date = dayDate,
                    option = "Non-Vegan"
                };
                await _menuDaysApi.CreateMenuDayAsync(nonVeganDay);
            }

            TempData["Success"] = "Semana criada com sucesso!";
            return RedirectToAction("MenuWeeks");
        }

        public async Task<IActionResult> EditWeek(int id)
        {
            var week = await _menuWeeksApi.GetMenuWeekAsync(id);
            if (week == null) return NotFound();

            var allDays = await _menuDaysApi.GetMenuDaysAsync();
            var weekDays = allDays
                .Where(d => d.menuweekid == week.Id)
                .OrderBy(d => d.date)
                .ThenBy(d => d.option)
                .ToList();

            ViewBag.WeekDays = weekDays;
            return View("MenuWeeks/Edit", week);
        }

        [HttpPost]
        public async Task<IActionResult> EditWeek(int id, List<MENUDAY> weekDays)
        {
            var week = await _menuWeeksApi.GetMenuWeekAsync(id);
            if (week == null) return NotFound();

            foreach (var day in weekDays)
            {
                if (day.date == null && !string.IsNullOrWhiteSpace(day.dateString))
                {
                    day.date = DateOnly.Parse(day.dateString);
                }

                if (day.id == 0)
                    await _menuDaysApi.CreateMenuDayAsync(day);
                else
                    await _menuDaysApi.UpdateMenuDayAsync(day);
            }

            return RedirectToAction("EditWeek", new { id });
        }
        public async Task<IActionResult> EditMenuDay(int id)
        {
            var day = await _menuDaysApi.GetMenuDayAsync(id);
            if (day == null)
                return NotFound();

            return View("MenuWeeks/EditMenuDay", day);
        }

        [HttpPost]
        public async Task<IActionResult> EditMenuDay(MENUDAY day)
        {
            if (day.date == null && !string.IsNullOrWhiteSpace(day.dateString))
            {
                day.date = DateOnly.Parse(day.dateString);
            }

            await _menuDaysApi.UpdateMenuDayAsync(day);

            return RedirectToAction("EditWeek", new { id = day.menuweekid });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWeek(int id)
        {
            var week = await _menuWeeksApi.GetMenuWeekAsync(id);
            if (week == null)
            {
                TempData["Error"] = "Week not found.";
                return RedirectToAction("MenuWeeks");
            }

            // Delete all days first
            foreach (var day in week.menudays ?? new List<MENUDAY>())
            {
                await _menuDaysApi.DeleteMenuDayAsync(day.id);
            }

            // Now delete the week
            var success = await _menuWeeksApi.DeleteMenuWeekAsync(id);
            if (!success) TempData["Error"] = "Failed to delete week.";

            return RedirectToAction("MenuWeeks");
        }
        // ---------------- CATEGORY CRUD ----------------
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoriesApi.GetCategoriesAsync();
            return View("Category/Index", categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("Category/CreateCategory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CATEGORIES model)
        {
            Console.WriteLine("✅ POST Admin/CreateCategory HIT");

            var success = await _categoriesApi.CreateCategoryAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "Erro ao criar categoria");
                return View("Category/CreateCategory",model);
            }

            return RedirectToAction("Categories");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int catid)
        {
            var category = await _categoriesApi.GetCategoryAsync(catid);
            if (category == null)
                return NotFound();

            return View("Category/EditCategory", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CATEGORIES model)
        {
            ModelState.Remove("products");

            if (!ModelState.IsValid)
            {
                return View("Category/EditCategory", model);
            }

            try
            {
                var success = await _categoriesApi.UpdateCategoryAsync(model);

                if (success)
                {
                    TempData["Success"] = "Categoria atualizada com sucesso!";
                    return RedirectToAction("Categories");
                }

                ModelState.AddModelError("", "O servidor da API rejeitou a atualização.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro técnico: {ex.Message}");
            }

            return View("Category/EditCategory", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int catid)
        {
            var success = await _categoriesApi.DeleteCategoryAsync(catid);
            if (!success)
            {
                TempData["Error"] = "Não foi possível apagar a categoria.";
                return RedirectToAction("Categories");
            }

            TempData["Success"] = "Categoria apagada com sucesso!";
            return RedirectToAction("Categories");
        }
        // ---------------- MATERIALS ----------------

        public async Task<IActionResult> Material()
        {
            var materials = await _materialsApi.GetMaterialsAsync();
            return View("Material/Index", materials);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMaterial()
        {
            ViewBag.Categories = await _materialCategoriesApi.GetMaterialCategoriesAsync();
            return View("Material/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaterial(MATERIALS model)
        {
            // Fix: Ensure Id is 0 so the DB generates a new one
            model.id = 0;
            ModelState.Remove("id");

            if (await _materialsApi.CreateMaterialAsync(model))
                return RedirectToAction("Material");

            ViewBag.Categories = await _materialCategoriesApi.GetMaterialCategoriesAsync();
            return View("Material/Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMaterial(int id)
        {
            var material = await _materialsApi.GetMaterialAsync(id);
            if (material == null) return NotFound();

            ViewBag.Categories = await _materialCategoriesApi.GetMaterialCategoriesAsync();
            return View("Material/Edit", material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaterial(MATERIALS model)
        {
            if (await _materialsApi.UpdateMaterialAsync(model))
                return RedirectToAction("Material");

            ViewBag.Categories = await _materialCategoriesApi.GetMaterialCategoriesAsync();
            return View("Material/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            await _materialsApi.DeleteMaterialAsync(id);
            return RedirectToAction("Material");
        }

        // ---------------- MATERIAL CATEGORIES ----------------

        public async Task<IActionResult> MaterialCategory()
        {
            var categories = await _materialCategoriesApi.GetMaterialCategoriesAsync();
            return View("MaterialCategory/Index", categories);
        }

        [HttpGet]
        public IActionResult CreateMaterialCategory() => View("MaterialCategory/Create");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaterialCategory(MATERIALCATEGORIES model)
        {
            // 1. Force Id to 0 so the Database Identity takes over
            model.Id = 0;

            // 2. Remove navigation properties/Ids from validation to prevent "Refresh"
            ModelState.Remove("Materials");
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
            {
                return View("MaterialCategory/Create", model);
            }

            var success = await _materialCategoriesApi.CreateMaterialCategoryAsync(model);
            if (success)
            {
                return RedirectToAction("MaterialCategory");
            }

            // If it still fails, the DB table likely isn't IDENTITY yet
            ModelState.AddModelError("", "API Error: The database rejected the save. Ensure the table 'id' column is set to IDENTITY.");
            return View("MaterialCategory/Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMaterialCategory(int id)
        {
            var category = await _materialCategoriesApi.GetMaterialCategoryAsync(id);
            if (category == null) return NotFound();
            return View("MaterialCategory/Edit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaterialCategory(MATERIALCATEGORIES model)
        {
            if (await _materialCategoriesApi.UpdateMaterialCategoryAsync(model))
                return RedirectToAction("MaterialCategory");

            return View("MaterialCategory/Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaterialCategory(int id)
        {
            await _materialCategoriesApi.DeleteMaterialCategoryAsync(id);
            return RedirectToAction("MaterialCategory");
        }
        // ---------------- Products CRUD ----------------
        public async Task<IActionResult> Products()
        {
            var prod = await _productsApi.GetProductsAsync();
            return View("Products/Index", prod);
        }
        public async Task<IActionResult> ProductsCreate()
        {
            ViewBag.Categories = await _categoriesApi.GetCategoriesAsync();
            return View("Products/Create");
        }
        [HttpPost]
        public async Task<IActionResult> ProductsCreate(PRODUCTS product)
        {
            await _productsApi.CreateProductAsync(product);
            var prod = await _productsApi.GetProductsAsync();
            return View("Products/Index", prod);
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productsApi.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
