using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public AssetsController(ApplicationDbContext context)
        {
                this.context = context;
        }



        // GET: api/<AssetsController>

        [Route("api/Assets/assets")]
        [HttpGet]
        
       
        public async Task<IActionResult> GetAssets()
        {

           var assets = await context.assets.ToListAsync();

            if (!assets.Any())
                return NotFound();

            return Ok(assets);
        }






       
    }
}
