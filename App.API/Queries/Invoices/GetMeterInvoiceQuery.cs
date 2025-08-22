using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using DocumentFormat.OpenXml.Office2016.Excel;
using MediatR;

namespace App.API.Queries.Invoices
{
    public class GetMeterInvoiceQuery : IRequest<InvoiceDetail>
    {
        public string MeterNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public GetMeterInvoiceQuery(string meterNumber, DateTime startDate, DateTime endDate)
        {

            //JSON naming
            //Tip dönüşümü 

            MeterNumber = meterNumber;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
