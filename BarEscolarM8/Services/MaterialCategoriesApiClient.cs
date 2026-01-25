//using System.Net.Http.Json;
//using APiConsumer.Models;

//namespace APiConsumer.Services
//{
//    public class MaterialCategoriesApiClient
//    {
//        private readonly HttpClient _httpClient;
//        public MaterialCategoriesApiClient(IHttpClientFactory httpClientFactory)
//        {
//            _httpClient = httpClientFactory.CreateClient("BarEscolaApi");
//        }

//        public async Task<List<MATERIALSC>> GetMaterialCategoriesAsync()
//        {
//            var result = await _httpClient.GetFromJsonAsync<List<MATERIALCATEGORIES>>("api/MATERIALCATEGORIES");
//            return result ?? new List<MATERIALCATEGORIES>();
//        }

//        public async Task<MATERIALCATEGORIES?> GetMaterialCategoryAsync(int id)
//        {
//            return await _httpClient.GetFromJsonAsync<MATERIALCATEGORIES>($"api/MATERIALCATEGORIES/{id}");
//        }

//        public async Task<bool> CreateMaterialCategoryAsync(MATERIALCATEGORIES category)
//        {
//            var response = await _httpClient.PostAsJsonAsync("api/MATERIALCATEGORIES", category);
//            return response.IsSuccessStatusCode;
//        }

//        public async Task<bool> UpdateMaterialCategoryAsync(MATERIALCATEGORIES category)
//        {
//            var response = await _httpClient.PutAsJsonAsync($"api/MATERIALCATEGORIES/{category.id}", category);
//            return response.IsSuccessStatusCode;
//        }

//        public async Task<bool> DeleteMaterialCategoryAsync(int id)
//        {
//            var response = await _httpClient.DeleteAsync($"api/MATERIALCATEGORIES/{id}");
//            return response.IsSuccessStatusCode;
//        }
//    }
//}