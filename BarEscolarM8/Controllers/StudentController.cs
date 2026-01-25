using APiConsumer.Models;
using APiConsumer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarEscolarM8.Controllers
{
    public class StudentController : Controller
    {
        private readonly MenuWeeksApiClient _menuWeeksApi;
        private readonly MenuDaysApiClient _menuDaysApi;
        private readonly ProductsApiClient _productsApi;
        private readonly CategoryApiClient _categoriesApi;
        private readonly OrderItemsApiClient _orderItemsApi;
        private readonly OrdersApiClient _ordersApi;
        public StudentController(
            MenuWeeksApiClient menuWeeksApi,
            MenuDaysApiClient menuDaysApi,
            ProductsApiClient productsApi,
            CategoryApiClient categoriesApi,
            OrderItemsApiClient orderitemsApi,
            OrdersApiClient orderApi)
        {
            _menuWeeksApi = menuWeeksApi;
            _menuDaysApi = menuDaysApi;
            _productsApi = productsApi;
            _categoriesApi = categoriesApi;
            _orderItemsApi = orderitemsApi;
            _ordersApi = orderApi;
        }
        public async Task<IActionResult> Index(string option = "A")
        {
            var allWeeks = await _menuWeeksApi.GetMenuWeeksAsync();
            var weeks = allWeeks
                .Select(week => new MENUWEEK
                {
                    Id = week.Id,
                    weekstart = week.weekstart,
                    menudays = week.menudays
                        .Where(m => m.date >= DateOnly.FromDateTime(DateTime.Today) && m.option == option)
                        .OrderBy(m => m.date)
                        .ToList()
                })
                .Where(w => w.menudays.Any())
                .ToList();

            ViewBag.SelectedOption = option;
            ViewBag.Categories = await _categoriesApi.GetCategoriesAsync();
            ViewBag.Users = User;

            return View(weeks);
        }
    }
}