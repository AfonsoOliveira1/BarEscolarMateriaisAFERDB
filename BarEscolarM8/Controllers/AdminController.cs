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
            if (!ModelState.IsValid)
            {
                var weekReload = await _menuWeeksApi.GetMenuWeekAsync(id);
                ViewBag.WeekDays = weekDays ?? new List<MENUDAY>();
                return View("MenuWeeks/Edit", weekReload);
            }

            var week = await _menuWeeksApi.GetMenuWeekAsync(id);
            if (week == null) return NotFound();

            // Update the week itself if needed
            await _menuWeeksApi.UpdateMenuWeekAsync(week);

            foreach (var day in weekDays)
            {
                // Parse date string from form back into DateOnly
                if (day.date == null && !string.IsNullOrWhiteSpace(day.dateString))
                {
                    if (DateOnly.TryParse(day.dateString, out var parsedDate))
                    {
                        day.date = parsedDate;
                    }
                }

                // If it’s a new day
                if (day.id == 0)
                {
                    await _menuDaysApi.CreateMenuDayAsync(day);
                }
                else
                {
                    await _menuDaysApi.UpdateMenuDayAsync(day);
                }
            }

            return RedirectToAction("MenuWeeks");
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


        // ---------------- MENU DAYS ----------------
        public IActionResult CreateDay(int weekId)
        {
            var day = new MENUDAY
            {
                menuweekid = weekId,
                date = DateOnly.FromDateTime(DateTime.Today)
            };
            return View(day);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDay(MENUDAY day)
        {
            await _menuDaysApi.CreateMenuDayAsync(day);
            return RedirectToAction("MenuWeeks");
        }
    }
}
