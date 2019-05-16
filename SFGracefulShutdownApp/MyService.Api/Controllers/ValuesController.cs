using System;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly StatelessServiceContext _context;

        public ValuesController(StatelessServiceContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]int delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            return Ok($"Hello from My Service on Node: {_context.NodeContext.NodeName}");
        }
    }
}
