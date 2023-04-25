using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
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
        OpenAIAPI api = new OpenAIAPI(new APIAuthentication(DotNetEnv.Env.GetString("OPEN_AI_API")));

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        private TelegramBotClient bot = TelegramBot.GetTelegramBot();

        [HttpPost]
        public async void Post([FromBody] Update update) //Update receiver method
        {
            var chat = api.Chat.CreateConversation();
            /// give instruction as System
            chat.AppendSystemMessage("You are a mental health doctor who helps anyone solve the problems with mental health. Your name is Alex. Be respectful to users, and ask questions only related to mental health.");

            long chatId = update.Message.Chat.Id;
            await bot.SendTextMessageAsync(chatId, $"Hello {update.Message.Chat.Username}! Welcome to AI Mental Health support. How are you doing? Tell me a bit about yourself");

            chat.AppendUserInput(update.Message.Text);
            string response = await chat.GetResponseFromChatbotAsync();

        }
        [HttpGet]
        public string Get()
        {
            //Init point
            return "Telegram bot was started";
        }
    }
}

