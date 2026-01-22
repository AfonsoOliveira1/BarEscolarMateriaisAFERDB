using APiConsumer.Models;
using APiConsumer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarEscolar.Controllers
{
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
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            var weeks = await _menuWeeksApi.GetMenuWeeksAsync();
            var products = await _productsApi.GetProductsAsync();
            var categories = await _categoriesApi.GetCategoriesAsync();

            ViewBag.Weeks = weeks;
            ViewBag.Products = products;
            ViewBag.Categories = categories;
            return View();
        }

        // ---------------- USERS MANAGEMENT ----------------
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            var users = await _usersApi.GetUsersAsync();
            return View(users); // criar IndexUsers.cshtm
        }

        public IActionResult CreateUser()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string fullName, string email, string username, string password, int role)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            var newUser = new USERS
            {
                fullname = fullName,
                email = email,
                username = username,
                role = role
            };

            var (Success, Error) = await _usersApi.CreateUserAsync(newUser, password);

            if (!Success) // if API call failed
                return BadRequest($"Erro ao criar usuário: {Error}");

            return RedirectToAction("Users");
        }


        // ---------------- MENU WEEKS ----------------
        public async Task<IActionResult> MenuWeeks()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            var weeks = await _menuWeeksApi.GetMenuWeeksAsync();
            return View(weeks);
        }

        public IActionResult CreateWeek()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWeek(DateTime weekStart)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            var week = new MENUWEEK
            {
                weekstart = weekStart.ToString("yyyy-MM-dd")
            };

            await _menuWeeksApi.CreateMenuWeekAsync(week);
            return RedirectToAction("MenuWeeks");
        }

        // ---------------- MENU DAYS ----------------
        public async Task<IActionResult> CreateDay(int weekId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

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
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            await _menuDaysApi.CreateMenuDayAsync(day);
            return RedirectToAction("MenuWeeks");
        }

        // ---------------- PRODUCTS ----------------
        public async Task<IActionResult> Products()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            var products = await _productsApi.GetProductsAsync();
            var categories = await _categoriesApi.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View(products);
        }

        // ---------------- CATEGORIES ----------------
        public async Task<IActionResult> Categories()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Login");

            var categories = await _categoriesApi.GetCategoriesAsync();
            return View(categories);
        }

        // ---------------- HELPER ----------------
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
            return role == 2; // 0 = Student, 1 = Employee, 2 = Admin
        }
    }
}
