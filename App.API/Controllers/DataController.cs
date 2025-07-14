using App.Services.Data;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("load")]
        public async Task<ActionResult<DataLoadResult>> LoadData([FromBody] string filePath)
        {
            var result = await _dataService.LoadDataFromExcelAsync(filePath);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("status")]
        public async Task<ActionResult<bool>> GetDataStatus()
        {
            var isLoaded = await _dataService.IsDataLoadedAsync();
            return Ok(isLoaded);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<DataLoadResult>> GetStatistics()
        {
            var result = await _dataService.GetDataStatisticsAsync();
            return Ok(result);
        }

        [HttpGet("meters")]
        public async Task<ActionResult> GetMeters()
        {
            var meters = await _dataService.GetMetersAsync();
            return Ok(meters);
        }

        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.Now,
                Version = "1.0.0"
            });
        }

        [HttpDelete("clear")]
        public async Task<ActionResult> ClearData()
        {
            await _dataService.ClearDataAsync();
            return Ok("Veriler temizlendi");
        }
    }
}
