using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BarEscolarM8.Controllers
{
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public StudentController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<IActionResult> Index(string option = "A")
        {
            var client = _clientFactory.CreateClient("APIBarescola");
            var token = User.FindFirst("JWToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("api/MenuWeek");

            return View(response);
        }
    }
}