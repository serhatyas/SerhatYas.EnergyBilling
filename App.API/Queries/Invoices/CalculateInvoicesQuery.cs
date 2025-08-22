using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using MediatR;

namespace App.API.Queries.Invoices
{
    public class CalculateInvoicesQuery : IRequest<InvoiceCalculationResponse>
    {
        public List<string> MeterNumbers { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public CalculateInvoicesQuery(List<string> meterNumbers, DateTime startDate, DateTime endDate)
        {

            //JSON naming
            //Tip dönüşümü 

            MeterNumbers = meterNumbers ?? new List<string>();
            StartDate = startDate;
            EndDate = endDate;
        }

        public CalculateInvoicesQuery(InvoiceCalculationRequest request)
        {
            MeterNumbers = request.MeterNumbers ?? new List<string>();
            StartDate = request.StartDate;
            EndDate = request.EndDate;
        }
    }
}
