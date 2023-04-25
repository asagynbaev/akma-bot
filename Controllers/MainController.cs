using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MindMate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Post(Update update) //Update receiver method
        {
            if (update != null)
            {
                if (update.Message != null)
                {
                    _logger.LogInformation(update.Message.Text);
                    Console.WriteLine(update.Message.Text);
                }
                else
                    _logger.LogInformation("update.Message is null");
            }
            else
                _logger.LogInformation("update is null");
        }
        [HttpGet]
        public string Get()
        {
            //Init point
            return "Telegram bot was started";
        }
    }
}



