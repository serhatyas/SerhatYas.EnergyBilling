using App.API.Queries.Invoices;
using App.Services.Invoices.SerhatYas.EnergyBilling.App.Services.Invoices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<InvoiceCalculationResponse>> CalculateInvoices([FromBody] InvoiceCalculationRequest request)
        {
            var result = await _mediator.Send(new CalculateInvoicesQuery(request));
            return Ok(result);
        }

        [HttpGet("{meterNumber}/calculate")]
        public async Task<ActionResult<InvoiceDetail>> CalculateMeterInvoice(
            [FromRoute] string meterNumber,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var query = new GetMeterInvoiceQuery(meterNumber, startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
