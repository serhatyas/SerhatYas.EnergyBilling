using App.Repositories.Consumptions;
using App.Repositories.Meters;
using App.Repositories.PriceInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Data
{
    public class DataLoadResult
    {
        // Başarılı mı
        public bool Success { get; set; }

        // Mesaj
        public string Message { get; set; } = string.Empty;

        // Yüklenen kayıt sayıları
        public int MetersLoaded { get; set; }
        public int ConsumptionsLoaded { get; set; }
        public int PriceInfosLoaded { get; set; }

        // Hata detayları
        public List<string> Errors { get; set; } = new();
    }

    public interface IDataService
    {
        // Excel dosyasından veri yükle
        Task<DataLoadResult> LoadDataFromExcelAsync(string filePath);

        // Sistem durumunu kontrol et
        Task<bool> IsDataLoadedAsync();

        // Veri istatistikleri
        Task<DataLoadResult> GetDataStatisticsAsync();

        // Sayaçları getir
        Task<List<Meter>> GetMetersAsync();

        // Tüketim verilerini getir
        Task<List<Consumption>> GetConsumptionsAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Fiyat bilgilerini getir
        Task<List<PriceInfo>> GetPriceInfosAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Veriyi temizle
        Task ClearDataAsync();
    }
}
