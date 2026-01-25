using APiConsumer.Models;
using APiConsumer.Services;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public AdminController(
            MenuWeeksApiClient menuWeeksApi,
            MenuDaysApiClient menuDaysApi,
            ProductsApiClient productsApi,
            CategoryApiClient categoriesApi,
            UsersApiClient usersApi,
            MaterialsApiClient materialsApi)
        {
            _menuWeeksApi = menuWeeksApi;
            _menuDaysApi = menuDaysApi;
            _productsApi = productsApi;
            _categoriesApi = categoriesApi;
            _usersApi = usersApi;
            _materialsApi = materialsApi;
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
        public IActionResult Categories()
        {
            var categories = await _categoriesApi.GetCategoriesAsync();
            return View("Category/Index", categories);
        }

        public IActionResult CreateCategory(string id)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserID");
            var user = _userStore.FindById(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");
            ViewBag.User = user;
            if (userName == null || userRole != "Admin" || userId != user.ID)
                return RedirectToAction("Login", "Login");

            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category, IFormFile imageFile, string id)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images");
                Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, imageFile.FileName);
                if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".gif")
                    || filePath.EndsWith("jpeg"))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    category.ImagePath = imageFile.FileName;
                }
                else
                {
                    return NotFound();
                }
            }
            category.Id = _categoryStore.GetAll().Any() ? _categoryStore.GetAll().Max(c => c.Id) + 1 : 1;
            _categoryStore.Add(category);
            return RedirectToAction("Categories", new { id = id });
        }

        public IActionResult EditCategory(int catid, string id)
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserID");
            var user = _userStore.FindById(id);
            ViewBag.User = user;
            if (user == null)
                return NotFound("Usuário não encontrado.");
            if (userName == null || userRole != "Admin" || userId != user.ID)
                return RedirectToAction("Login", "Login");

            var category = _categoryStore.FindById(catid);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(Category category, IFormFile imageFile, string userid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images");
                Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, imageFile.FileName);
                if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".gif")
                    || filePath.EndsWith("jpeg"))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    category.ImagePath = imageFile.FileName;
                }
                else
                {
                    return NotFound();
                }
            }
            _categoryStore.Update(category);
            return RedirectToAction("Categories", new { id = userid });
        }

        [HttpPost]
        public IActionResult DeleteCategory(int catid, string id)
        {
            _categoryStore.Delete(catid);
            return RedirectToAction("Categories", new { id = id });
        }
        // ---------------- Material ----------------
        public async Task<IActionResult> Material()
        {
            var material = await _materialsApi.GetMaterialsAsync();
            return View("Material/Index", material);
        }

        // ---------------- Materials Category ----------------
        public async Task<IActionResult> MaterialCategory()
        {
            var materialcategory = await _menuWeeksApi.GetMenuWeeksAsync();
            return View("MenuWeeks/Index", materialcategory);
        }
    }
}
