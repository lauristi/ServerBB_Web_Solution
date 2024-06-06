using ServerBB_Web.Service.Model;

namespace ServerBB_Web.Service.Interface
{
    public interface ISpending
    {
        Task<Spending> GetSpendingAsync();
    }
}