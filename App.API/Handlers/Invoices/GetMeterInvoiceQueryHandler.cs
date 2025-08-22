using App.API.Queries.Invoices;
using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using MediatR;

namespace App.API.Handlers.Invoices
{
    public class GetMeterInvoiceQueryHandler : IRequestHandler<GetMeterInvoiceQuery, InvoiceDetail>
    {
        private readonly IInvoiceCalculationService _invoiceCalculationService;

        public GetMeterInvoiceQueryHandler(IInvoiceCalculationService invoiceCalculationService)
        {
            _invoiceCalculationService = invoiceCalculationService;
        }

        public async Task<InvoiceDetail> Handle(GetMeterInvoiceQuery request, CancellationToken cancellationToken)
        {
            // Query'yi service request'ine çevir
            //Cache
            //Logging 
            //Authorization
            //validation

            return await _invoiceCalculationService.CalculateMeterInvoiceAsync(
                request.MeterNumber,
                request.StartDate,
                request.EndDate);
        }
    }
}
