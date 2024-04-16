using Microsoft.AspNetCore.Mvc;

namespace CarModelService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelsController : ControllerBase
    {

        private readonly ILogger<ModelsController> _logger;

        public ModelsController(ILogger<ModelsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetModels(int modelyear, string make)
        {

            return Ok();
        }
    }
}