using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Invoices
{
    namespace SerhatYas.EnergyBilling.App.Services.Invoices
    {
        public class InvoiceCalculationRequest
        {
            // Başlangıç Tarihi
            public DateTime StartDate { get; set; }

            // Bitiş Tarihi
            public DateTime EndDate { get; set; }

            // Sayaç Numaraları (boş ise tümü)
            public List<string> MeterNumbers { get; set; } = new();
        }

        public class InvoiceDetail
        {
            // Sayaç Numarası
            public string MeterNumber { get; set; } = string.Empty;

            // Belediye
            public string Municipality { get; set; } = string.Empty;

            // Toplam Tüketim (MWh)
            public decimal TotalConsumption { get; set; }

            // Enerji Bedeli (TL)
            public decimal EnergyAmount { get; set; }

            // Dağıtım Bedeli (TL)
            public decimal DistributionAmount { get; set; }

            // BTV Tutarı (TL)
            public decimal MunicipalityTaxAmount { get; set; }

            // Ara Toplam (TL)
            public decimal SubTotal { get; set; }

            // KDV Tutarı (TL)
            public decimal VatAmount { get; set; }

            // Toplam Fatura Tutarı (TL)
            public decimal TotalAmount { get; set; }

            // Saatlik Detaylar
            public List<HourlyDetail> HourlyDetails { get; set; } = new();
        }

        public class HourlyDetail
        {
            // Tarih ve Saat
            public DateTime DateTime { get; set; }

            // Tüketim (MWh)
            public decimal Consumption { get; set; }

            // PTF Fiyatı (TL/MWh)
            public decimal PtfPrice { get; set; }

            // Enerji Bedeli (TL)
            public decimal Amount { get; set; }
        }

        public class InvoiceCalculationResponse
        {
            // Fatura Detayları
            public List<InvoiceDetail> InvoiceDetails { get; set; } = new();

            // Toplam Fatura Tutarı
            public decimal TotalInvoiceAmount { get; set; }

            // Toplam BTV Tutarı
            public decimal TotalMunicipalityTaxAmount { get; set; }

            // Hesaplama Tarihi
            public DateTime CalculationDate { get; set; } = DateTime.Now;
        }

        public interface IInvoiceCalculationService
        {
            // Fatura hesaplama
            Task<InvoiceCalculationResponse> CalculateInvoicesAsync(InvoiceCalculationRequest request);

            // Tek sayaç faturası
            Task<InvoiceDetail> CalculateMeterInvoiceAsync(string meterNumber, DateTime startDate, DateTime endDate);
        }
    }
}
