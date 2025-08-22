using App.Repositories.Constants;
using App.Repositories.Consumptions;
using App.Repositories.Enums;
using App.Repositories.Meters;
using App.Repositories.PriceInfos;
using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using DocumentFormat.OpenXml.Office2016.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Invoices
{

    // Sayaç ve tüketim verilerine göre fatura tutarını hesaplar.
    // Komisyon, indirim, KDV ve BTV dahil tüm kalemleri değerlendirir.

    // Fatura hesaplama servisi

    public class InvoiceCalculationService
        : IInvoiceCalculationService
    {
        private readonly IMeterRepository _meterRepository;
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly IPriceInfoRepository _priceInfoRepository;

        public InvoiceCalculationService(
            IMeterRepository meterRepository,
            IConsumptionRepository consumptionRepository,
            IPriceInfoRepository priceInfoRepository)
        {
            _meterRepository = meterRepository;
            _consumptionRepository = consumptionRepository;
            _priceInfoRepository = priceInfoRepository;
        }

        // Fatura hesaplama
        //tüm sayaçları hesaplar
        public async Task<InvoiceCalculationResponse> CalculateInvoicesAsync(InvoiceCalculationRequest request)
        {

            if (request.EndDate < request.StartDate)
                throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz");

            if (request.MeterNumbers != null && request.MeterNumbers.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Sayaç numarası boş olamaz");

            var response = new InvoiceCalculationResponse();

            // Sayaçları getir
            var meters = await GetFilteredMetersAsync(request.MeterNumbers);

            // Her sayaç için fatura hesapla
            foreach (var meter in meters)
            {
                var invoiceDetail = await CalculateMeterInvoiceAsync(meter.MeterNumber, request.StartDate, request.EndDate);
                response.InvoiceDetails.Add(invoiceDetail);
            }

            // Toplamları hesapla
            response.TotalInvoiceAmount = response.InvoiceDetails.Sum(i => i.TotalAmount);
            response.TotalMunicipalityTaxAmount = response.InvoiceDetails.Sum(i => i.MunicipalityTaxAmount);

            return response;
        }

        // Tek sayaç faturası
        public async Task<InvoiceDetail> CalculateMeterInvoiceAsync(string meterNumber, DateTime startDate, DateTime endDate)
        {

            if (endDate < startDate)
                throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz");

            if (string.IsNullOrWhiteSpace(meterNumber))
                throw new ArgumentException("Sayaç numarası boş olamaz");

            // Sayaç bilgisini getir
            var meter = await _meterRepository.GetByMeterNumberAsync(meterNumber);
            if (meter == null)
                throw new ArgumentException($"Meter {meterNumber} not found");

            // Tüketim verilerini getir
            var consumptions = await _consumptionRepository.GetByMeterAndDateRangeAsync(meterNumber, startDate, endDate);

            // Fiyat bilgilerini getir
            var priceInfos = await _priceInfoRepository.GetByDateRangeAsync(startDate, endDate);

            var invoiceDetail = new InvoiceDetail
            {
                MeterNumber = meterNumber,
                Municipality = meter.Municipality,
                TotalConsumption = consumptions.Sum(c => c.ConsumptionAmount)
            };

            // Enerji bedeli hesapla ((ptf + yek)*s1,s2,s3tüketim(mwh))
            var energyCalculation = CalculateEnergyAmount(meter, consumptions.ToList(), priceInfos.ToList());
            invoiceDetail.EnergyAmount = energyCalculation.totalAmount;
            invoiceDetail.HourlyDetails = energyCalculation.hourlyDetails;

            // Tarifeye göre Dağıtım bedeli hesapla (Sanayi, Ticarethane)
            invoiceDetail.DistributionAmount = CalculateDistributionAmount(meter, invoiceDetail.TotalConsumption);

            // BTV hesapla
            // Enerji bedeli üzerinden BTV oranı uygulanır (örnek: 1% veya 5%)
            invoiceDetail.MunicipalityTaxAmount = invoiceDetail.EnergyAmount * meter.MunicipalityTaxRate;

            // Ara toplam (Enerji + Dağıtım)
            // Bu aşamada BTV ve KDV henüz eklenmemiştir.
            invoiceDetail.SubTotal = invoiceDetail.EnergyAmount + invoiceDetail.DistributionAmount;

            // KDV hesapla (ara toplam * KDV oranı)
            invoiceDetail.VatAmount = invoiceDetail.SubTotal * meter.VatRate;

            // Toplam fatura (Enerji + Dağıtım + BTV + KDV)
            // BTV dahil değildir → BTV ayrı gösterilir.
            // TotalAmount = AraToplam + KDV
            invoiceDetail.TotalAmount = invoiceDetail.SubTotal + invoiceDetail.VatAmount;

            return invoiceDetail;
        }

        // Helper metodlar
        private async Task<List<Meter>> GetFilteredMetersAsync(List<string> meterNumbers)
        {
            var allMeters = await _meterRepository.GetAllAsync();

            if (meterNumbers.Any())
                return allMeters.Where(m => meterNumbers.Contains(m.MeterNumber)).ToList();

            return allMeters.ToList();
        }

        private (decimal totalAmount, List<HourlyDetail> hourlyDetails) CalculateEnergyAmount(
            Meter meter, List<Consumption> consumptions, List<PriceInfo> priceInfos)
        {
            var totalAmount = 0m;
            var hourlyDetails = new List<HourlyDetail>();

            foreach (var consumption in consumptions)
            {
                var consumptionDateTime = consumption.Date.AddHours(consumption.Hour - 1);
                var priceInfo = priceInfos.FirstOrDefault(p =>
                    p.DateTime.Date == consumptionDateTime.Date &&
                    p.DateTime.Hour == consumptionDateTime.Hour);

                var ptfPrice = priceInfo?.PtfPrice ?? 0;
                var hourlyAmount = 0m;

                switch (meter.SalesMethod)
                {
                    case SalesMethod.PtfYekWithPercentageCommission:
                        // (PTF + YEK) * Tüketim * (1 + Komisyon%)
                        hourlyAmount = (ptfPrice + PriceConstants.YekPrice) * consumption.ConsumptionAmount *
                                      (1 + (meter.CommissionRate ?? 0));
                        break;

                    case SalesMethod.PtfYekWithFixedCommission:
                        // (PTF + YEK + Sabit Komisyon) * Tüketim
                        hourlyAmount = (ptfPrice + PriceConstants.YekPrice + (meter.FixedCommissionAmount ?? 0)) *
                                      consumption.ConsumptionAmount;
                        break;

                    case SalesMethod.TariffWithPercentageDiscount:
                        // Tarife fiyatı * Tüketim * (1 - İndirim%)
                        var tariffPrice = meter.TariffType == TariffType.Industrial ?
                                         PriceConstants.IndustrialEnergyTariff :
                                         PriceConstants.CommercialEnergyTariff;
                        hourlyAmount = tariffPrice * consumption.ConsumptionAmount * (1 - (meter.DiscountRate ?? 0));
                        break;
                }

                totalAmount += hourlyAmount;

                hourlyDetails.Add(new HourlyDetail
                {
                    DateTime = consumptionDateTime,
                    Consumption = consumption.ConsumptionAmount,
                    PtfPrice = ptfPrice,
                    Amount = hourlyAmount
                });
            }

            return (totalAmount, hourlyDetails);
        }

        private decimal CalculateDistributionAmount(Meter meter, decimal totalConsumption)
        {
            var distributionPrice = meter.TariffType == TariffType.Industrial ?
                                   PriceConstants.IndustrialDistributionTariff :
                                   PriceConstants.CommercialDistributionTariff;

            return distributionPrice * totalConsumption;
        }
    }
}
