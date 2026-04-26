using dog_dojo_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace dog_dojo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeeklyQuestController : ControllerBase
    {
        private readonly IQuestService _questService;

        public WeeklyQuestController(IQuestService questService)
        {
            _questService = questService;
        }


        [HttpGet("current-quest")]
        public async Task<IActionResult> GetCurrentQuest()
        {
            var quest = await _questService.GetCurrentQuestAsync();
            return Ok(quest);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshQuestIfPossible()
        {
            if (await _questService.CheckIfNewQuestNeededAsync())
            {
                await _questService.RefreshQuestAsync();
            }
            return Ok();
        }
    }
}
