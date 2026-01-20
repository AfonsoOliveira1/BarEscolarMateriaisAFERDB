using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class OrdersApiClient
    {
        private readonly HttpClient _httpClient;
        public OrdersApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<ORDERS>> GetOrdersAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ORDERS>>("api/ORDERS");
            return result ?? new List<ORDERS>();
        }

        public async Task<ORDERS?> GetOrderAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ORDERS>($"api/ORDERS/{id}");
        }

        public async Task<bool> CreateOrderAsync(ORDERS order)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ORDERS", order);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateOrderAsync(ORDERS order)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ORDERS/{order.id}", order);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ORDERS/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}