using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class MaterialsApiClient
    {
        private readonly HttpClient _httpClient;
        public MaterialsApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<MATERIALS>> GetMaterialsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<MATERIALS>>("api/MATERIALS");
                return result ?? new List<MATERIALS>();
            }
            catch
            {
                return new List<MATERIALS>();
            }
        }

        public async Task<MATERIALS?> GetMaterialAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MATERIALS>($"api/MATERIALS/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateMaterialAsync(MATERIALS material)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MATERIALS", material);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateMaterialAsync(MATERIALS material)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/MATERIALS/{material.id}", material);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/MATERIALS/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}