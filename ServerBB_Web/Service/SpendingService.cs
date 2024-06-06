using ServerBB_Web.Service.Model;

namespace ServerBB_Web.Service
{
    public class SpendingService
    {
        private readonly HttpClient _httpClient;

        public SpendingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Spending> GetSpendingAsync()
        {
            return await _httpClient.GetFromJsonAsync<Spending>("api/bb/spending");
        }
    }
}