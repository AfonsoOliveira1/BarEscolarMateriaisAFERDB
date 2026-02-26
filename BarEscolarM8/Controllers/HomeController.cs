using System.Diagnostics;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Mvc;

namespace BarEscolarM8.Controllers
{
    public class HomeController : Controller
    {
        public HomeController() { }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
