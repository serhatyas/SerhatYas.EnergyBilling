using App.Repositories.Consumptions;
using App.Repositories.PriceInfos;
using System.Diagnostics.Metrics;

namespace App.Repositories.Data
{
    public interface IExcelDataReader
    {
        // Sayaç bilgilerini oku
        Task<List<App.Repositories.Meters.Meter>> ReadMetersAsync();

        // Tüketim bilgilerini oku
        Task<List<Consumption>> ReadConsumptionsAsync();

        // Fiyat bilgilerini oku
        Task<List<PriceInfo>> ReadPriceInfosAsync();

        // Dosya yolu ayarla
        void SetFilePath(string filePath);

        // Dosya var mı kontrol et
        bool FileExists();
    }
}
