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
            var result = await _httpClient.GetFromJsonAsync<List<CATEGORIES>>("api/Category");
            return result ?? new List<CATEGORIES>();
        }

        // Get a single category by ID
        public async Task<CATEGORIES?> GetCategoryAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Category/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CATEGORIES>();
        }

        // Create a new category
        public async Task<bool> CreateCategoryAsync(CATEGORIES category)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Category", category);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return true;
        }

        // Update an existing category
        public async Task<bool> UpdateCategoryAsync(CATEGORIES category)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Category/{category.Id}", category);
            return response.IsSuccessStatusCode;
        }


        // Delete a category by ID
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Category/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
