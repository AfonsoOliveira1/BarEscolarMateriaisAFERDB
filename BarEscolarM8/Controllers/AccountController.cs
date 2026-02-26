using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using BarEscolarM8.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Identity.Data;

namespace BarEscolarM8.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("APIBarescola");

            var response = await client.PostAsJsonAsync("api/User/login", model);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

                if (loginResponse?.User != null)
                {
                    var user = loginResponse.User;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role?.ToString().ToUpper() ?? "STUDENT"),
                        new Claim("FullName", user.FullName),
                        new Claim("JWToken", loginResponse.Token),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                    );

                    TempData["Message"] = "Bem-vindo de volta!";

                    if (user.Role == "STUDENT")
                        return RedirectToAction("Index", "Student");

                    if (user.Role == "EMPLOYEE")
                        return RedirectToAction("Index", "Employee");

                    if (user.Role == "ADMIN")
                        return RedirectToAction("Index", "Admin");

                }
            }

            ModelState.AddModelError("", "Email/User ou password incorretos.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("APIBarescola");

            var userDto = new UserCreateDto
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Passwordhash,
                Role = model.Role,
                FullName = model.FullName,
            };

            var response = await client.PostAsJsonAsync("api/User/register", userDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Registo efetuado! Por favor faça login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Falha no registo. Verifique se o email já está em uso.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            //  limpar Cookie 
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return View("Login");
        }
    }
}