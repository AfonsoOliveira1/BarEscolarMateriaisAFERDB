using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using APiConsumer.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace APiConsumer.Services
{
    public class AuthenticationService
    {
        private readonly UsersApiClient _usersApi;
        private readonly PasswordHasher<USERS> _passwordHasher;

        public AuthenticationService(UsersApiClient usersApi)
        {
            _usersApi = usersApi;
            _passwordHasher = new PasswordHasher<USERS>();
        }

        // ---------- LOGIN ----------
        public async Task<USERS?> AuthenticateAsync(string emailOrUsername, string password)
        {
            var users = await _usersApi.GetUsersAsync();

            var user = users.FirstOrDefault(u =>
                u.username.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                u.email.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.passwordhash,
                password
            );

            return result == PasswordVerificationResult.Success ? user : null;
        }

        // ---------- REGISTER ----------
        public async Task<(bool Success, string? Error)> RegisterAsync(
            string fullName,
            string username,
            string email,
            string password)
        {
            var users = await _usersApi.GetUsersAsync();

            if (users.Any(u => u.username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return (false, "Username already taken.");

            if (users.Any(u => u.email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return (false, "Email already registered.");

            var newUser = new USERS
            {
                id = Guid.NewGuid().ToString(),
                fullname = fullName,
                username = username,
                email = email,
                role = 0 // Student by default
            };

            newUser.passwordhash = _passwordHasher.HashPassword(newUser, password);

            var (ok, error) = await _usersApi.CreateUserAsync(newUser);
            if (!ok)
                return (false, error);

            return (true, null);
        }

        // ---------- CLAIMS ----------
        public Task<ClaimsPrincipal> CreatePrincipalAsync(USERS user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id ?? ""),
                new Claim(ClaimTypes.Name, user.username ?? "")
            };

            string roleName = user.role switch
            {
                0 => "Student",
                1 => "Employee",
                2 => "Admin",
                _ => "Student"
            };

            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return Task.FromResult(new ClaimsPrincipal(identity));
        }
    }
}
