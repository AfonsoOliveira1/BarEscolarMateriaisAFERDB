using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class MenuDaysApiClient
    {
        private readonly HttpClient _httpClient;
        public MenuDaysApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<MENUDAY>> GetMenuDaysAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<MENUDAY>>("api/MENUDAYS");
            return result ?? new List<MENUDAY>();
        }

        public async Task<MENUDAY?> GetMenuDayAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<MENUDAY>($"api/MENUDAYS/{id}");
        }

        public async Task<bool> CreateMenuDayAsync(MENUDAY day)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MENUDAYS", day);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateMenuDayAsync(MENUDAY day)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/MENUDAYS/{day.id}", day);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMenuDayAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/MENUDAYS/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}