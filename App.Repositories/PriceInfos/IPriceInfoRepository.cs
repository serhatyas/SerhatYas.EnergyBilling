using App.Repositories.Base;

namespace App.Repositories.PriceInfos
{
    public interface IPriceInfoRepository:IGenericRepository<PriceInfo>
    {       
        // Tarih aralığına göre getir
        Task<IEnumerable<PriceInfo>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Tarihe göre getir
        Task<PriceInfo?> GetByDateTimeAsync(DateTime dateTime);

        // Ortalama fiyat hesapla
        Task<decimal> GetAveragePriceAsync(DateTime startDate, DateTime endDate);
    }
}
