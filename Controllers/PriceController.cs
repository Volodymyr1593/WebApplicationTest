using Microsoft.AspNetCore.Mvc;
using WebApplicationTest.Types;



namespace WebApplicationTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {


        private readonly ApplicationDbContext context;
        private readonly IFintachartsService fintachartsService;
        public PriceController(ApplicationDbContext context, IFintachartsService fintachartsService)
        {
            this.context = context;
            this.fintachartsService = fintachartsService;
        }


        [HttpGet]
        public async Task<IActionResult> GetPrices([FromQuery] string[] assets)
        {
            if (assets == null || assets.Length == 0)
                return BadRequest("Missing 'assets' query parameter.");

            var prices = await fintachartsService.GetPricesRealTime(assets);
            return Ok(prices);
        }


    }
}
