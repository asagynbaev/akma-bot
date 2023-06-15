using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MindMate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly DialogContext _context;
        OpenAIAPI api = new OpenAIAPI(new APIAuthentication(DotNetEnv.Env.GetString("OPEN_AI_API")));

        public MainController(ILogger<MainController> logger, DialogContext context)
        {
            _logger = logger;
            _context = context;
        }

        private TelegramBotClient bot = TelegramBot.GetTelegramBot();

        [HttpPost("talk")]
        public async Task Post([FromBody] Update update) //Update receiver method
        {
            try
            {
                _logger.LogInformation($"Message from user: {update.Message.Text}");

                var conversation = api.Chat.CreateConversation();
                long chatId = update.Message.Chat.Id;

                if (update.Message.Text != null)
                {
                    // Check if this is a first interaction
                    if (update.Message.Text == "/start")
                    {
                        conversation.AppendUserInput($@"Ты - Роза, Вы врач-психиатр, который помогает
                            любому человеку решить проблемы с психическим здоровьем. Тебя зовут Роза.
                            Проявляйте уважение к пользователям и задавайте вопросы, связанные только с психическим здоровьем.");

                        await bot.SendTextMessageAsync(chatId, $@"Привет {update.Message.Chat.Username}!
                            Добро пожаловать в службу поддержки психического здоровья с помощью искусственного интеллекта.Меня зовут Роза.
                            Как дела? Давай познакомимся поближе! Скажи мне, как тебя зовут и сколько тебе лет.");
                    }
                    else
                    {
                        string result = await TelegramBot.DoConversation(chatId, conversation, update.Message.Text);

                        // Запись диалога в базу данных
                        var dialog = new Dialog
                        {
                            UserId = update.Message.Chat.Username,
                            UserMessage = update.Message.Text,
                            BotResponse =  result,
                            Timestamp = DateTime.UtcNow
                        };

                        _context.Dialogs.Add(dialog);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        [HttpGet("init")]
        public string InitChatGPT()
        {
            // ChatGPT initialization
            var chat = api.Chat.CreateConversation();

            chat.AppendSystemMessage($@"Ты - Роза, Вы врач-психиатр, который помогает
                любому человеку решить проблемы с психическим здоровьем. Тебя зовут Роза.
                Проявляйте уважение к пользователям и задавайте вопросы, связанные только с психическим здоровьем.");

            return "Successfully initialized";
        }
    }
}
