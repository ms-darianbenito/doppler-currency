using System;
using System.Threading.Tasks;
using Doppler.Currency.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Doppler.Currency.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsdCurrencyController : ControllerBase
    {
        private readonly ILogger<UsdCurrencyController> _logger;
        private readonly IBnaService _bnaService;

        public UsdCurrencyController(ILogger<UsdCurrencyController> logger, IBnaService bnaService) => 
            (_logger, _bnaService) = (logger, bnaService);

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? date = null)
        {
            if (date > DateTime.UtcNow)
            {
                return BadRequest("Date is not valid, please check again with format MM/dd/yyyy");
            }

            _logger.LogInformation("Getting Usd today");
            var result = await _bnaService.GetUsdToday(date);

            if (result.Success)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}
