using App.Repositories.Enums;

namespace App.Repositories.Meters
{
    public class Meter
    {

        //exceldeki sayaç bilgileri
        
        // Sayaç numarası     
        public string MeterNumber { get; set; } = string.Empty;

        
        // Satış yöntemi türü    
        public SalesMethod SalesMethod { get; set; }

        // Tarife Türü
        public TariffType TariffType { get; set; }

        // Komisyon oranı    
        public decimal? CommissionRate { get; set; }

        
        // Sabit komisyon tutarı        
        public decimal? FixedCommissionAmount { get; set; }

        
        // İndirim oranı       
        public decimal? DiscountRate { get; set; }

        
        // Belediye Tüketim Vergisi oranı
        public decimal MunicipalityTaxRate { get; set; }

        
        // Katma Değer Vergisi oranı
        public decimal VatRate { get; set; }
        
        // Tarife adı       
        public string TariffName { get; set; } = string.Empty;

        
        // Belediye adı    
        public string Municipality { get; set; } = string.Empty;
    }
}
