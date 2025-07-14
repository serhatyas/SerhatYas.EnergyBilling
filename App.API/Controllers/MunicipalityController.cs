using App.Services.Municipalitie;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MunicipalityController : ControllerBase
    {
        private readonly IMunicipalityPaymentService _municipalityPaymentService;

        public MunicipalityController(IMunicipalityPaymentService municipalityPaymentService)
        {
            _municipalityPaymentService = municipalityPaymentService;
        }

        [HttpPost("payments")]
        public async Task<ActionResult<List<MunicipalityPayment>>> CalculatePayments([FromBody] MunicipalityPaymentRequest request)
        {
            var result = await _municipalityPaymentService.CalculatePaymentsAsync(request);
            return Ok(result);
        }

        [HttpGet("{municipality}/payment")]
        public async Task<ActionResult<MunicipalityPayment>> GetMunicipalityPayment(
            [FromRoute] string municipality,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var result = await _municipalityPaymentService.GetMunicipalityPaymentAsync(municipality, startDate, endDate);

            if (result == null)
                return NotFound($"Belediye bulunamadı: {municipality}");

            return Ok(result);
        }

        [HttpGet("total-tax")]
        public async Task<ActionResult<decimal>> GetTotalTaxAmount([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _municipalityPaymentService.GetTotalTaxAmountAsync(startDate, endDate);
            return Ok(result);
        }
    }
}
