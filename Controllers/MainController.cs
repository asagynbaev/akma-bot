using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindMate.Entities;
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
                        conversation.AppendSystemMessage(DotNetEnv.Env.GetString("BOT_CONTEXT_SETTINGS"));

                        var ifExists = await _context.Patients.SingleAsync(x => x.TelegramUserId == update.Message.Chat.Id);

                        // Check if user exists, if not, put it in database
                        if(ifExists == null)
                        {
                            Patient patient = new Patient()
                            {
                                Username = update.Message.Chat.Username,
                                TelegramUserId = update.Message.Chat.Id,
                                Firstname = update.Message.Chat.FirstName,
                                Lastname = update.Message.Chat.LastName,
                                CreatedAt = DateTime.UtcNow
                            };

                            _context.Patients.Add(patient);
                            await _context.SaveChangesAsync();
                        }

                        await bot.SendTextMessageAsync(chatId, $@"Привет {update.Message.Chat.Username}! Добро пожаловать в службу поддержки ментального здоровья. Меня зовут Люссид. Как твои дела? Расскажи мне немного о том, с чем ты столкнулся/сталкиваешься в своей жизни, что привело тебя сюда?");
                    }
                    else
                    {
                        string newresult = await TelegramBot.DoConversation(chatId, conversation, update.Message.Text);

                        var dialog = new Dialog
                        {
                            Username = update.Message.Chat.Username,
                            UserMessage = update.Message.Text,
                            BotResponse =  newresult,
                            Timestamp = DateTime.UtcNow,
                            TelegramUserId = update.Message.Chat.Id.ToString()
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
        
        [HttpGet("send-notification")]
        public async Task SendReminderMessage()
        {
            List<Patient> patients = await _context.Patients.ToListAsync();
            var message = "";
            foreach(var item in patients)
            {
                string newresult = await TelegramBot.DoConversation(item.TelegramUserId, message);
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
                conversation.AppendSystemMessage(DotNetEnv.Env.GetString("BOT_CONTEXT_SETTINGS"));
                _userConversations.TryAdd(chatId, conversation);
                return conversation;
            }
        }
    }
}
