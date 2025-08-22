using App.API.Queries.Invoices;
using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using DocumentFormat.OpenXml.Drawing;
using MediatR;

namespace App.API.Handlers.Invoices
{
    public class CalculateInvoicesQueryHandler : IRequestHandler<CalculateInvoicesQuery, InvoiceCalculationResponse>
    {
        private readonly IInvoiceCalculationService _invoiceCalculationService;

        public CalculateInvoicesQueryHandler(IInvoiceCalculationService invoiceCalculationService)
        {
            _invoiceCalculationService = invoiceCalculationService;
        }

        public async Task<InvoiceCalculationResponse> Handle(CalculateInvoicesQuery request, CancellationToken cancellationToken)
        {
            // Query'yi service request'ine çevir
            //Cache
            //Logging 
            //Authorization
            //validation

            var serviceRequest = new InvoiceCalculationRequest
            {
                MeterNumbers = request.MeterNumbers,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            return await _invoiceCalculationService.CalculateInvoicesAsync(serviceRequest);
        }
    }
}
