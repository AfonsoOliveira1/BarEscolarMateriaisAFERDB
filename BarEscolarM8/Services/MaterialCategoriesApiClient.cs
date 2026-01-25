using System.Net.Http.Json;
using APiConsumer.Models;
using BarEscolarM8.Models;

namespace APiConsumer.Services
{
    public class MaterialCategoriesApiClient
    {
        private readonly HttpClient _httpClient;

        public MaterialCategoriesApiClient(IHttpClientFactory httpClientFactory)
        {
            // IMPORTANT: Ensure "APIBarEscola" matches the string in Program.cs
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<MATERIALCATEGORIES>> GetMaterialCategoriesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<MATERIALCATEGORIES>>("api/MATERIALCATEGORIES");
                return result ?? new List<MATERIALCATEGORIES>();
            }
            catch { return new List<MATERIALCATEGORIES>(); }
        }

        public async Task<MATERIALCATEGORIES?> GetMaterialCategoryAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MATERIALCATEGORIES>($"api/MATERIALCATEGORIES/{id}");
            }
            catch { return null; }
        }

        public async Task<bool> CreateMaterialCategoryAsync(MATERIALCATEGORIES category)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MATERIALCATEGORIES", category);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Reject: {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateMaterialCategoryAsync(MATERIALCATEGORIES category)
        {
            // Fix: Changed 'category.id' to 'category.Id'
            var response = await _httpClient.PutAsJsonAsync($"api/MATERIALCATEGORIES/{category.Id}", category);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMaterialCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/MATERIALCATEGORIES/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}