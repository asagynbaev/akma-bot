using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Chat;
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
        private static readonly ConcurrentDictionary<long, Conversation> _userConversations = new ConcurrentDictionary<long, Conversation>();
        private readonly OpenAIAPI _openAIAPI;
        private readonly TelegramBotClient _telegramBotClient;

        public MainController(ILogger<MainController> logger, DialogContext context)
        {
            _logger = logger;
            _context = context;
            _openAIAPI = new OpenAIAPI(new APIAuthentication(DotNetEnv.Env.GetString("OPEN_AI_API")));
            _telegramBotClient = TelegramBot.GetTelegramBot();
        }

        private TelegramBotClient bot = TelegramBot.GetTelegramBot();

        [HttpPost("talk")]
        public async Task Post([FromBody] Update update) //Update receiver method
        {
            try
            { 
                long chatId = update.Message.Chat.Id;
                
                var conversation = GetOrCreateConversation(chatId);

                if (update.Message.Text != null)
                {
                    // Check if this is a first interaction
                    if (update.Message.Text == "/start")
                    {
                        conversation.AppendSystemMessage($@"Ты - Люссид, Вы врач-психолог, который помогает
                            любому человеку решить проблемы с ментальным здоровьем. Тебя зовут Люссид. Ты девушка.
                            Задавай вопросы, которые помогут углубить разговор. Старайся не просто раздавать 
                            советы. Необходимо, чтоб ты задавала уточняющие вопросы, которые помогут выявить симптомы
                            и то сколько человек уже живет с данной проблемой. Помни, ты Роза - и твоя задача 
                            поддерживать беседу и собирать информацию, которую можно будет передать в дальнейшем врачу.");

                        await bot.SendTextMessageAsync(chatId, $@"Привет {update.Message.Chat.Username}! Добро пожаловать в службу поддержки ментального здоровья. Меня зовут Люссид. Как твои дела? Расскажи мне немного о том, с чем ты столкнулся/сталкиваешься в своей жизни, что привело тебя сюда?");
                    }
                    else
                    {
                        string newresult = await TelegramBot.DoConversation(chatId, conversation, update.Message.Text);

                        // Запись диалога в базу данных
                        var dialog = new Dialog
                        {
                            UserId = update.Message.Chat.Username,
                            UserMessage = update.Message.Text,
                            BotResponse =  newresult,
                            Timestamp = DateTime.UtcNow,
                            TelegramUserId = update.Message.Chat.Id
                        };

                        _context.Dialogs.Add(dialog);
                        _context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        private Conversation GetOrCreateConversation(long chatId)
        {
            if (_userConversations.TryGetValue(chatId, out var conversation))
            {
                return conversation;
            }
            else
            {
                conversation = _openAIAPI.Chat.CreateConversation();
                conversation.AppendSystemMessage($@"Ты - Люссид, Вы врач-психолог, который помогает
                    любому человеку решить проблемы с ментальным здоровьем. Тебя зовут Люссид. Ты девушка.
                    Задавай вопросы, которые помогут углубить разговор. Старайся не просто раздавать 
                    советы. Необходимо, чтоб ты задавала уточняющие вопросы, которые помогут выявить симптомы
                    и то сколько человек уже живет с данной проблемой. Помни, ты Роза - и твоя задача 
                    поддерживать беседу и собирать информацию, которую можно будет передать в дальнейшем врачу.");

                _userConversations.TryAdd(chatId, conversation);
                return conversation;
            }
        }
    }
}
