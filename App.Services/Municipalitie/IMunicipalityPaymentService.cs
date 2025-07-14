using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Municipalitie
{
    public class MunicipalityPayment
    {
        // Belediye Adı
        public string Municipality { get; set; } = string.Empty;

        // Toplam BTV Tutarı
        public decimal TotalAmount { get; set; }

        // Sayaç Detayları
        public List<MeterTaxDetail> MeterDetails { get; set; } = new();
    }

    public class MeterTaxDetail
    {
        // Sayaç Numarası
        public string MeterNumber { get; set; } = string.Empty;

        // BTV Tutarı
        public decimal TaxAmount { get; set; }

        // BTV Oranı
        public decimal TaxRate { get; set; }
    }

    public class MunicipalityPaymentRequest
    {
        // Başlangıç Tarihi
        public DateTime StartDate { get; set; }

        // Bitiş Tarihi
        public DateTime EndDate { get; set; }

        // Sayaç Numaraları (boş ise tümü)
        public List<string> MeterNumbers { get; set; } = new();
    }

    public interface IMunicipalityPaymentService
    {
        // Belediye ödemelerini hesapla
        Task<List<MunicipalityPayment>> CalculatePaymentsAsync(MunicipalityPaymentRequest request);

        // Tek belediye ödemesi
        Task<MunicipalityPayment?> GetMunicipalityPaymentAsync(string municipality, DateTime startDate, DateTime endDate);

        // Toplam BTV tutarı
        Task<decimal> GetTotalTaxAmountAsync(DateTime startDate, DateTime endDate);
    }
}
