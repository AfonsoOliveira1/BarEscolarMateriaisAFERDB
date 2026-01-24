using APiConsumer.Models;
using APiConsumer.Services;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public AdminController(
            MenuWeeksApiClient menuWeeksApi,
            MenuDaysApiClient menuDaysApi,
            ProductsApiClient productsApi,
            CategoryApiClient categoriesApi,
            UsersApiClient usersApi)
        {
            _menuWeeksApi = menuWeeksApi;
            _menuDaysApi = menuDaysApi;
            _productsApi = productsApi;
            _categoriesApi = categoriesApi;
            _usersApi = usersApi;
        }

        // ---------------- DASHBOARD ----------------
        public async Task<IActionResult> Index()
        {
            ViewBag.Weeks = await _menuWeeksApi.GetMenuWeeksAsync();
            ViewBag.Products = await _productsApi.GetProductsAsync();
            ViewBag.Categories = await _categoriesApi.GetCategoriesAsync();

            return View();
        }

        // ---------------- USERS ----------------
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
        public async Task<IActionResult> CreateUser(string fullName,string email,string username,string password, int role)
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

            return RedirectToAction("Users");
        }
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _usersApi.GetUserAsync(id);

            if (user == null)
                return NotFound();

            return View("Users/EditUser", user);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser( string id, string fullName,string email,string username,string? password,int role)
        {
            var user = await _usersApi.GetUserAsync(id);
            if (user == null)
                return NotFound();

            user.fullname = fullName;
            user.email = email;
            user.username = username;
            user.role = role;

            // only update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.passwordhash = password;
            }

            var success = await _usersApi.UpdateUserAsync(user);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to update user");
                return View("Users/EditUser", user);
            }

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

            return RedirectToAction("Users");
        }


        // ---------------- MENU WEEKS ----------------
        public async Task<IActionResult> MenuWeeks()
        {
            // List all weeks
            var weeks = await _menuWeeksApi.GetMenuWeeksAsync();
            return View("MenuWeeks", weeks); 
        }

        public IActionResult CreateWeek()
        {
            return View("MenuWeeks/Create",new CreateWeekViewModel { WeekStart = DateTime.Today });
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

            var success = await _menuWeeksApi.CreateMenuWeekAsync(week);

            if (!success)
            {
                ModelState.AddModelError("", "Falha ao criar a semana.");
                ViewBag.EnteredDate = weekStart.ToString("yyyy-MM-dd");
                return View("MenuWeeks/Create");
            }

            TempData["Success"] = "Semana criada com sucesso!";
            return RedirectToAction("MenuWeeks"); 
        }




        public async Task<IActionResult> EditWeek(int id)
        {
            var week = await _menuWeeksApi.GetMenuWeekAsync(id);
            if (week == null) return NotFound();

            return View(week); 
        }

        [HttpPost]
        public async Task<IActionResult> EditWeek(MENUWEEK week)
        {
            if (!ModelState.IsValid) return View(week);

            var success = await _menuWeeksApi.UpdateMenuWeekAsync(week);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to update menu week.");
                return View(week);
            }

            return RedirectToAction("MenuWeeks");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWeek(int id)
        {
            var success = await _menuWeeksApi.DeleteMenuWeekAsync(id);

            if (!success) TempData["Error"] = "Failed to delete week.";

            return RedirectToAction("MenuWeeks");
        }


        // ---------------- MENU DAYS ----------------
        public IActionResult CreateDay(int weekId)
        {
            var day = new MENUDAY
            {
                menuweekid = weekId,
                date = DateOnly.FromDateTime(DateTime.Today).ToString()
            };

            return View(day);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDay(MENUDAY day)
        {
            await _menuDaysApi.CreateMenuDayAsync(day);
            return RedirectToAction("MenuWeeks");
        }

        // ---------------- PRODUCTS ----------------
        public async Task<IActionResult> Products()
        {
            ViewBag.Categories = await _categoriesApi.GetCategoriesAsync();
            var products = await _productsApi.GetProductsAsync();
            return View(products);
        }

        // ---------------- CATEGORIES ----------------
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoriesApi.GetCategoriesAsync();
            return View(categories);
        }
    }
}
