using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UsdQuotation.Services;

namespace UsdQuotation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsdQuotationController : ControllerBase
    {
        private readonly ILogger<UsdQuotationController> _logger;
        private readonly IBnaService _bnaService;

        public UsdQuotationController(ILogger<UsdQuotationController> logger, IBnaService bnaService)
        {
            _logger = logger;
            _bnaService = bnaService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Getting Usd today");
            var result = await _bnaService.GetUsdToday();

            if (result != null)
                return Ok(result);

            return NotFound("You can not found the USD quotation today");
        }
    }
}
