using System.Net.Http.Json;
using APiConsumer.Models;

namespace APiConsumer.Services
{
    public class OrderItemsApiClient
    {
        private readonly HttpClient _httpClient;
        public OrderItemsApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIBarEscola");
        }

        public async Task<List<ORDERITEMS>> GetOrderItemsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ORDERITEMS>>("api/ORDERITEMS");
            return result ?? new List<ORDERITEMS>();
        }

        public async Task<ORDERITEMS?> GetOrderItemAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ORDERITEMS>($"api/ORDERITEMS/{id}");
        }

        public async Task<bool> CreateOrderItemAsync(ORDERITEMS item)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ORDERITEMS", item);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateOrderItemAsync(ORDERITEMS item)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ORDERITEMS/{item.id}", item);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrderItemAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ORDERITEMS/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}