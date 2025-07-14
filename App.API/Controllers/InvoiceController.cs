using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceCalculationService _invoiceCalculationService;

        public InvoiceController(IInvoiceCalculationService invoiceCalculationService)
        {
            _invoiceCalculationService = invoiceCalculationService;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<InvoiceCalculationResponse>> CalculateInvoices([FromBody] InvoiceCalculationRequest request)
        {
            if (request.EndDate < request.StartDate)
                return BadRequest("Bitiş tarihi başlangıç tarihinden önce olamaz");

            var result = await _invoiceCalculationService.CalculateInvoicesAsync(request);
            return Ok(result);
        }

        [HttpGet("{meterNumber}/calculate")]
        public async Task<ActionResult<InvoiceDetail>> CalculateMeterInvoice(
            [FromRoute] string meterNumber,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _invoiceCalculationService.CalculateMeterInvoiceAsync(meterNumber, startDate, endDate);
            return Ok(result);
        }
    }
}
