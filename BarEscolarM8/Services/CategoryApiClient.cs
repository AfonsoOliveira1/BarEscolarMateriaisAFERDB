using System.Net.Http.Json;
using APiConsumer.Models;
using BarEscolarM8.Models;

namespace APiConsumer.Services
{
    public class CategoryApiClient
    {
        private readonly HttpClient _httpClient;

        public CategoryApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        // Get all categories
        public async Task<List<CATEGORIES>> GetCategoriesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<CATEGORIES>>("api/Categories");
            return result ?? new List<CATEGORIES>();
        }

        // Get a single category by ID
        public async Task<CATEGORIES?> GetCategoryAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CATEGORIES>($"api/Categories/{id}");
        }

        // Create a new category
        public async Task<bool> CreateCategoryAsync(CATEGORIES category)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Categories", category);
            return response.IsSuccessStatusCode;
        }

        // Update an existing category
        public async Task<bool> UpdateCategoryAsync(CATEGORIES category)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Categories/{category.id}", category);
            return response.IsSuccessStatusCode;
        }

        // Delete a category by ID
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Categories/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
