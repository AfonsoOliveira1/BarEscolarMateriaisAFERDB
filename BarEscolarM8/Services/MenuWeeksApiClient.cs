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
            var result = await _httpClient.GetFromJsonAsync<List<MENUWEEK>>("api/MenuWeek");
            return result ?? new List<MENUWEEK>();
        }

        public async Task<MENUWEEK?> GetMenuWeekAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<MENUWEEK>($"api/MenuWeek/{id}");
        }

        public async Task<MENUWEEK?> CreateMenuWeekAsync(MENUWEEK week)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MenuWeek", week);
            if (!response.IsSuccessStatusCode)
                return null;

            var createdWeek = await response.Content.ReadFromJsonAsync<MENUWEEK>();
            return createdWeek;
        }

        public async Task<bool> UpdateMenuWeekAsync(MENUWEEK week)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/MenuWeek/{week.Id}", week);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMenuWeekAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/MenuWeek/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}