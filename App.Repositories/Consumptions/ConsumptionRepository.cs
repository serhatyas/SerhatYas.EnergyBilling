using App.Repositories.Base;

namespace App.Repositories.Consumptions
{

    // Bellekteki tüketim (consumption) verilerine erişim sağlar.
    // Sayaç numarasına ve döneme göre tüketim verilerini getirir.

    // s1 s2 s3 tüketimleri getirir
    public class ConsumptionRepository : GenericRepository<Consumption>, IConsumptionRepository
    {
        public ConsumptionRepository(List<Consumption> consumptions) : base(consumptions)
        {

        }

        // Sayaç ve tarih aralığına göre getir
        public async Task<IEnumerable<Consumption>> GetByMeterAndDateRangeAsync(string meterNumber, DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(_data.Where(c =>
                c.MeterNumber == meterNumber &&
                c.Date >= startDate.Date &&
                c.Date <= endDate.Date));
        }

        // Tarih aralığına göre getir
        public async Task<IEnumerable<Consumption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(_data.Where(c =>
                c.Date >= startDate.Date &&
                c.Date <= endDate.Date));
        }

        // Toplam tüketim hesapla
        public async Task<decimal> GetTotalConsumptionAsync(string meterNumber, DateTime startDate, DateTime endDate)
        {
            var consumptions = await GetByMeterAndDateRangeAsync(meterNumber, startDate, endDate);
            return consumptions.Sum(c => c.ConsumptionAmount);
        }

        // Sayaç numarasına göre getir
        public async Task<IEnumerable<Consumption>> GetByMeterNumberAsync(string meterNumber)
        {
            return await Task.FromResult(_data.Where(c => c.MeterNumber == meterNumber));
        }
    }
}
