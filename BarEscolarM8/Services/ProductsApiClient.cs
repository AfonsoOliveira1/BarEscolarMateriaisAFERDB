using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class ProductsApiClient
    {
        private readonly HttpClient _httpClient;
        public ProductsApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<PRODUCTS>> GetProductsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<PRODUCTS>>("api/PRODUCTS");
            return result ?? new List<PRODUCTS>();
        }

        public async Task<PRODUCTS?> GetProductAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<PRODUCTS>($"api/PRODUCTS/{id}");
        }

        public async Task<bool> CreateProductAsync(PRODUCTS product)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PRODUCTS", product);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(PRODUCTS product)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/PRODUCTS/{product.id}", product);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/PRODUCTS/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}