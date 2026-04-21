using Microsoft.AspNetCore.Mvc;

namespace dog_dojo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeeklyQuestController : ControllerBase
    {
        private readonly string _currentQuestFile = "current_quest.json";

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!System.IO.File.Exists(_currentQuestFile))
                return NotFound("No quest selected yet.");

            var json = await System.IO.File.ReadAllTextAsync(_currentQuestFile);

            return Content(json, "application/json");
        }
    }
}
