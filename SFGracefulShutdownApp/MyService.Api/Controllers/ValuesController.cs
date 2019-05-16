using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            return Ok("Hello from My Service");
        }
    }
}
