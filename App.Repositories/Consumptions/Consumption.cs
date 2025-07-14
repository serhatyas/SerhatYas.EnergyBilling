namespace App.Repositories.Consumptions
{

    // Sayaç tüketim bilgilerini temsil eden entity sınıfı
    public class Consumption
    {
        // Sayaç Numarası
        public string MeterNumber { get; set; } = string.Empty;

        // Tüketim Tarihi
        public DateTime Date { get; set; }

        // Saat Bilgisi
        public int Hour { get; set; }

        // Tüketim Miktarı (MWh)
        public decimal ConsumptionAmount { get; set; }
    }
}
