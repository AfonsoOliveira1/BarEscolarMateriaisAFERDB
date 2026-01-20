using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class HistoricosApiClient
    {
        private readonly HttpClient _httpClient;
        public HistoricosApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<HISTORICOS>> GetHistoricosAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<HISTORICOS>>("api/HISTORICOS");
            return result ?? new List<HISTORICOS>();
        }

        public async Task<HISTORICOS?> GetHistoricoAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<HISTORICOS>($"api/HISTORICOS/{id}");
        }

        public async Task<bool> CreateHistoricoAsync(HISTORICOS h)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HISTORICOS", h);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateHistoricoAsync(HISTORICOS h)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/HISTORICOS/{h.id}", h);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteHistoricoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/HISTORICOS/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}