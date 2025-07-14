using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Municipalitie
{
    public class MunicipalityPaymentService : IMunicipalityPaymentService
    {
        private readonly IInvoiceCalculationService _invoiceCalculationService;

        public MunicipalityPaymentService(IInvoiceCalculationService invoiceCalculationService)
        {
            _invoiceCalculationService = invoiceCalculationService;
        }

        // Belediye ödemelerini hesapla
        public async Task<List<MunicipalityPayment>> CalculatePaymentsAsync(MunicipalityPaymentRequest request)
        {
            // Önce faturaları hesapla
            var invoiceRequest = new InvoiceCalculationRequest
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MeterNumbers = request.MeterNumbers
            };

            var invoiceResponse = await _invoiceCalculationService.CalculateInvoicesAsync(invoiceRequest);

            // Belediye bazında grupla
            var municipalityPayments = invoiceResponse.InvoiceDetails
                .GroupBy(i => i.Municipality)
                .Select(g => new MunicipalityPayment
                {
                    Municipality = g.Key,
                    TotalAmount = g.Sum(i => i.MunicipalityTaxAmount),
                    MeterDetails = g.Select(i => new MeterTaxDetail
                    {
                        MeterNumber = i.MeterNumber,
                        TaxAmount = i.MunicipalityTaxAmount,
                        TaxRate = i.EnergyAmount > 0 ? i.MunicipalityTaxAmount / i.EnergyAmount : 0
                    }).ToList()
                })
                .ToList();

            return municipalityPayments;
        }

        // Tek belediye ödemesi
        public async Task<MunicipalityPayment?> GetMunicipalityPaymentAsync(string municipality, DateTime startDate, DateTime endDate)
        {
            var request = new MunicipalityPaymentRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                MeterNumbers = new List<string>()
            };

            var payments = await CalculatePaymentsAsync(request);
            return payments.FirstOrDefault(p =>
                p.Municipality.Equals(municipality, StringComparison.OrdinalIgnoreCase));
        }

        // Toplam BTV tutarı
        public async Task<decimal> GetTotalTaxAmountAsync(DateTime startDate, DateTime endDate)
        {
            var request = new MunicipalityPaymentRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                MeterNumbers = new List<string>()
            };

            var payments = await CalculatePaymentsAsync(request);
            return payments.Sum(p => p.TotalAmount);
        }
    }
}
