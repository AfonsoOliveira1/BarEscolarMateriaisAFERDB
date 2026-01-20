using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class AuthenticationService
    {
        private readonly UsersApiClient _usersApi;
        private readonly RolesApiClient _rolesApi;
        private readonly PasswordHasher<USERS> _passwordHasher;

        public AuthenticationService(UsersApiClient usersApi, RolesApiClient rolesApi)
        {
            _usersApi = usersApi;
            _rolesApi = rolesApi;
            _passwordHasher = new PasswordHasher<USERS>();
        }

        public async Task<USERS?> AuthenticateAsync(string emailOrUsername, string password)
        {
            var users = await _usersApi.GetUsersAsync();
            var user = users.FirstOrDefault(u =>
                string.Equals(u.username, emailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.email, emailOrUsername, StringComparison.OrdinalIgnoreCase));

            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.passwordhash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        // Now returns API error message when creation fails
        public async Task<(bool Success, string? Error)> RegisterAsync(string fullName, string username, string email, string password)
        {
            var users = await _usersApi.GetUsersAsync();

            if (users.Any(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase)))
                return (false, "Username already taken.");

            if (users.Any(u => string.Equals(u.email, email, StringComparison.OrdinalIgnoreCase)))
                return (false, "Email already registered.");

            // Get the role id from API (fallback to 1 if unavailable)
            int roleId = 1;
            try
            {
                var roleFromDb = await _rolesApi.GetRoleAsync(1);
                if (roleFromDb != null)
                    roleId = roleFromDb.id;
            }
            catch
            {
                // ignore and use fallback 1
            }

            var newUser = new USERS
            {
                id = Guid.NewGuid().ToString(),
                username = username,
                fullname = fullName,
                email = email,
                role = roleId
            };

            newUser.passwordhash = _passwordHasher.HashPassword(newUser, password);

            var (ok, apiError) = await _usersApi.CreateUserAsync(newUser);
            if (!ok)
                return (false, apiError ?? "API error creating user.");

            return (true, null);
        }

        // CreatePrincipalAsync unchanged...
        public async Task<ClaimsPrincipal> CreatePrincipalAsync(USERS user)
        {
            string userName = user.username ?? string.Empty;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id ?? string.Empty),
                new Claim(ClaimTypes.Name, userName)
            };

            if (user.role.HasValue)
            {
                try
                {
                    var roleObj = await _rolesApi.GetRoleAsync(user.role.Value);
                    string roleName = roleObj?.role1 ?? user.role.Value.ToString();
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
                catch
                {
                    claims.Add(new Claim(ClaimTypes.Role, user.role.Value.ToString()));
                }
            }

            var identity = new ClaimsIdentity(claims, "cookies");
            return new ClaimsPrincipal(identity);
        }
    }
}