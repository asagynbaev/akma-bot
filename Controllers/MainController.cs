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

        private TelegramBotClient bot = TelegramBot.GetTelegramBot();

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update) //Update receiver method
        {
            long chatId = update.Message.Chat.Id;
            await bot.SendTextMessageAsync(chatId, $"Hello {update.Message.Chat.FirstName}! Welcome to AI Mental Health support.");

            if (update != null)
            {
                if (update.Message != null)
                {
                    _logger.LogInformation(update.Message.Text);
                    Console.WriteLine(update.Message.Text);
                    return Ok($"Your message is: {update.Message.Text}");
                }
                else
                {
                    _logger.LogInformation("update.Message is null");
                    return Ok($"update.Message is null");
                }
            }
            else
            {
                _logger.LogInformation("update is null");
                return Ok($"update.Message is null");
            }
        }
        [HttpGet]
        public string Get()
        {
            //Init point
            return "Telegram bot was started";
        }
    }
}



