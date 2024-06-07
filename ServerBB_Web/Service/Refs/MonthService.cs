using ServerBB_Web.Service.Interface;
using ServerBB_Web.Service.Model;
using System.Globalization;

namespace ServerBB_Web.Service.Refs
{
    public class MonthService : IMonthService
    {
        public List<Month> GetMonths()
        {
            return Enumerable.Range(1, 12)
                .Select(x => new Month { Id = x, Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x).ToUpper() })
                .ToList();
        }
    }
}