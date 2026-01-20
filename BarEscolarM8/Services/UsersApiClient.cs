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

        // NOW returns (Success, ErrorMessage) so caller can know why API failed
        public async Task<(bool Success, string? Error)> CreateUserAsync(USERS user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User", user);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

                // read error body (may contain modelstate/errors)
                string body = string.Empty;
                try
                {
                    body = await response.Content.ReadAsStringAsync();
                }
                catch { /* ignore read errors */ }

                // prefer body if available, otherwise include status
                var errorMsg = !string.IsNullOrWhiteSpace(body)
                    ? $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}"
                    : $"{(int)response.StatusCode} {response.ReasonPhrase}";

                return (false, errorMsg);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error: {ex.Message}");
            }
        }

        // keep others simple (you can expand similarly)
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