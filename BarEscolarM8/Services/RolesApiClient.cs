using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class RolesApiClient
    {
        private readonly HttpClient _httpClient;
        public RolesApiClient(IHttpClientFactory httpClientFactory)
        {
            // Deve coincidir com o nome registado em Program.cs
            _httpClient = httpClientFactory.CreateClient("ApiBarEscola");
        }

        public async Task<List<ROLES>> GetRolesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ROLES>>("api/Role");
                return result ?? new List<ROLES>();
            }
            catch
            {
                return new List<ROLES>();
            }
        }

        public async Task<ROLES?> GetRoleAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ROLES>($"api/Role/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateRoleAsync(ROLES role)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Role", role);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRoleAsync(ROLES role)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Role/{role.id}", role);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Role/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}