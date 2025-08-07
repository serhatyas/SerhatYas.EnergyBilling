using App.Repositories.Consumptions;
using App.Repositories.Data;
using App.Repositories.Meters;
using App.Repositories.PriceInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Data
{
    // API veya UI katmanına veri sağlayan servis katmanıdır.
    // Sayaç listesi, tüketim detayları ve fiyat bilgilerini birleştirip sunar.

    public class DataService : IDataService
    {
        private readonly IExcelDataReader _excelDataReader;
        private readonly IMeterRepository _meterRepository;
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly IPriceInfoRepository _priceInfoRepository;

        public DataService(
            IExcelDataReader excelDataReader,
            IMeterRepository meterRepository,
            IConsumptionRepository consumptionRepository,
            IPriceInfoRepository priceInfoRepository)
        {
            _excelDataReader = excelDataReader;
            _meterRepository = meterRepository;
            _consumptionRepository = consumptionRepository;
            _priceInfoRepository = priceInfoRepository;
        }

        // Excel dosyasından veri yükle
        public async Task<DataLoadResult> LoadDataFromExcelAsync(string filePath)
        {
            var result = new DataLoadResult();

            try
            {
                // Dosya kontrolü
                if (!File.Exists(filePath))
                {
                    result.Success = false;
                    result.Message = "Excel dosyası bulunamadı";
                    result.Errors.Add($"File not found: {filePath}");
                    return result;
                }

                // Excel reader'ı ayarla
                _excelDataReader.SetFilePath(filePath);

                // Verileri oku
                var meters = await _excelDataReader.ReadMetersAsync();
                var consumptions = await _excelDataReader.ReadConsumptionsAsync();
                var priceInfos = await _excelDataReader.ReadPriceInfosAsync();

                // Repository'lere yükle
                await _meterRepository.AddRangeAsync(meters);
                await _consumptionRepository.AddRangeAsync(consumptions);
                await _priceInfoRepository.AddRangeAsync(priceInfos);

                // Sonuç bilgilerini doldur
                result.Success = true;
                result.Message = "Veriler başarıyla yüklendi";
                result.MetersLoaded = meters.Count;
                result.ConsumptionsLoaded = consumptions.Count;
                result.PriceInfosLoaded = priceInfos.Count;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Veri yükleme sırasında hata oluştu";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        // Sistem durumunu kontrol et
        public async Task<bool> IsDataLoadedAsync()
        {

            //Bellekte sayaç bilgileri yüklendiğini kontrol eder.

            var meters = await _meterRepository.GetAllAsync();
            return meters.Any();
        }

        // Veri istatistikleri
        public async Task<DataLoadResult> GetDataStatisticsAsync()
        {

            // Yüklenen sayaç, tüketim ve fiyat bilgisi adedini döner.
            var result = new DataLoadResult
            {
                Success = true,
                Message = "Veri istatistikleri",
                MetersLoaded = (await _meterRepository.GetAllAsync()).Count(),
                ConsumptionsLoaded = (await _consumptionRepository.GetAllAsync()).Count(),
                PriceInfosLoaded = (await _priceInfoRepository.GetAllAsync()).Count()
            };

            return result;
        }

        // Sayaçları getir
        public async Task<List<Meter>> GetMetersAsync()
        {
            // tüm sayaçları getirir.

            var meters = await _meterRepository.GetAllAsync();
            return meters.ToList();
        }

        // Tüketim verilerini getir
        public async Task<List<Consumption>> GetConsumptionsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Tarihe göre filtreli ya da tüm tüketimleri getirir.

            if (startDate.HasValue && endDate.HasValue)
            {
                var consumptions = await _consumptionRepository.GetByDateRangeAsync(startDate.Value, endDate.Value);
                return consumptions.ToList();
            }

            var allConsumptions = await _consumptionRepository.GetAllAsync();
            return allConsumptions.ToList();
        }

        // Fiyat bilgilerini getir
        public async Task<List<PriceInfo>> GetPriceInfosAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            //Tarihe göre filtreli ya da tüm fiyat bilgilerini getirir.

            if (startDate.HasValue && endDate.HasValue)
            {
                var priceInfos = await _priceInfoRepository.GetByDateRangeAsync(startDate.Value, endDate.Value);
                return priceInfos.ToList();
            }

            var allPriceInfos = await _priceInfoRepository.GetAllAsync();
            return allPriceInfos.ToList();
        }

        // Veriyi temizle
        public async Task ClearDataAsync()
        {
            // Memory repository için özel temizleme gerekebilir
            // Şimdilik boş bırakıyoruz
            await Task.CompletedTask;
        }
    }
}
