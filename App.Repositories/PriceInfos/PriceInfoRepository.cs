using App.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.PriceInfos
{

    // Bellekteki fiyat bilgilerine erişim sağlar.
    // Tarife, kWh başına fiyat, BTV, KDV gibi bilgileri içerir.

    //Fiyat bilgilerinde ptf fiyatları getirir
    public class PriceInfoRepository : GenericRepository<PriceInfo>, IPriceInfoRepository
    {
        public PriceInfoRepository(List<PriceInfo> priceInfos) : base(priceInfos)
        {
        }

        // Tarih aralığına göre getir
        public async Task<IEnumerable<PriceInfo>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(_data.Where(p =>
                p.DateTime >= startDate &&
                p.DateTime <= endDate));
        }

        // Tarihe göre getir
        public async Task<PriceInfo?> GetByDateTimeAsync(DateTime dateTime)
        {
            return await Task.FromResult(_data.FirstOrDefault(p =>
                p.DateTime.Date == dateTime.Date &&
                p.DateTime.Hour == dateTime.Hour));
        }

        // Ortalama fiyat hesapla
        public async Task<decimal> GetAveragePriceAsync(DateTime startDate, DateTime endDate)
        {
            var priceInfos = await GetByDateRangeAsync(startDate, endDate);
            return priceInfos.Any() ? priceInfos.Average(p => p.PtfPrice) : 0;
        }
    }
}

