using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class UsersApiClient
    {
        private readonly HttpClient _httpClient;
        public UsersApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<USERS>> GetUsersAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<USERS>>("api/User");
                return result ?? new List<USERS>();
            }
            catch
            {
                return new List<USERS>();
            }
        }

        public async Task<USERS?> GetUserAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<USERS>($"api/User/{id}");
            }
            catch
            {
                return null;
            }
        }

        // ORIGINAL: CreateUserAsync sending just USERS object
        public async Task<(bool Success, string? Error)> CreateUserAsync(USERS user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User", user);

                if (response.IsSuccessStatusCode)
                    return (true, null);

                string body = string.Empty;
                try { body = await response.Content.ReadAsStringAsync(); } catch { }

                var errorMsg = !string.IsNullOrWhiteSpace(body)
                    ? $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}"
                    : $"{(int)response.StatusCode} {response.ReasonPhrase}";

                return (false, errorMsg);
            }
            catch (HttpRequestException ex) { return (false, $"Network error: {ex.Message}"); }
            catch (Exception ex) { return (false, $"Unexpected error: {ex.Message}"); }
        }

        // NEW: overload to create user with password
        public async Task<(bool Success, string? Error)> CreateUserAsync(USERS user, string password)
        {
            try
            {
                var payload = new
                {
                    user.fullname,
                    user.email,
                    user.username,
                    Password = password, // API handles hashing
                    user.role
                };

                var response = await _httpClient.PostAsJsonAsync("api/User", payload);

                if (response.IsSuccessStatusCode)
                    return (true, null);

                string body = string.Empty;
                try { body = await response.Content.ReadAsStringAsync(); } catch { }

                var errorMsg = !string.IsNullOrWhiteSpace(body)
                    ? $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}"
                    : $"{(int)response.StatusCode} {response.ReasonPhrase}";

                return (false, errorMsg);
            }
            catch (HttpRequestException ex) { return (false, $"Network error: {ex.Message}"); }
            catch (Exception ex) { return (false, $"Unexpected error: {ex.Message}"); }
        }

        public async Task<bool> UpdateUserAsync(USERS user)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/User/{user.id}", user);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/User/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
