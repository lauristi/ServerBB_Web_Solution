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
            try
            {
                return await _httpClient.GetFromJsonAsync<Spending>("api/bb/spending");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Impossível interagir com o endpoint", ex);
            }
        }
    }
}