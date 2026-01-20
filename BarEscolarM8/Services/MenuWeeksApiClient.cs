using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class MenuWeeksApiClient
    {
        private readonly HttpClient _httpClient;
        public MenuWeeksApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<MENUWEEK>> GetMenuWeeksAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<MENUWEEK>>("api/MENUWEEKS");
            return result ?? new List<MENUWEEK>();
        }

        public async Task<MENUWEEK?> GetMenuWeekAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<MENUWEEK>($"api/MENUWEEKS/{id}");
        }

        public async Task<bool> CreateMenuWeekAsync(MENUWEEK week)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MENUWEEKS", week);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateMenuWeekAsync(MENUWEEK week)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/MENUWEEKS/{week.id}", week);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMenuWeekAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/MENUWEEKS/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}