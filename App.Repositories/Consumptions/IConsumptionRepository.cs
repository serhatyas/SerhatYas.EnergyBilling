using App.Repositories.Base;
using App.Repositories.PriceInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Consumptions
{
    public interface IConsumptionRepository : IGenericRepository<Consumption>
    {
        // Sayaç ve tarih aralığına göre getir
        Task<IEnumerable<Consumption>> GetByMeterAndDateRangeAsync(string meterNumber, DateTime startDate, DateTime endDate);

        // Tarih aralığına göre getir
        Task<IEnumerable<Consumption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Toplam tüketim hesapla
        Task<decimal> GetTotalConsumptionAsync(string meterNumber, DateTime startDate, DateTime endDate);

        // Sayaç numarasına göre getir
        Task<IEnumerable<Consumption>> GetByMeterNumberAsync(string meterNumber);
    }
}
