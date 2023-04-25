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
        private Dictionary<long, Conversation>? values;
        OpenAIAPI api = new OpenAIAPI(new APIAuthentication(DotNetEnv.Env.GetString("OPEN_AI_API")));

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        private TelegramBotClient bot = TelegramBot.GetTelegramBot();

        [HttpPost("talk")]
        public async void Post([FromBody] Update update) //Update receiver method
        {
            if(update.Message.Text != null)
            {
                // Check if this is a first interaction
                if (update.Message.Text == "/start")
                {
                    long chatId = update.Message.Chat.Id;
                    await bot.SendTextMessageAsync(chatId, $@"Hello {update.Message.Chat.Username}!
                        Welcome to AI Mental Health support.My name is Alex.
                        How are you doing? Tell me a bit about yourself");
                }
                else
                {
                    long chatId = update.Message.Chat.Id;

                    if(values != null)
                    {
                        bool DictionaryHasChatId = values.ContainsKey(chatId);
                        if (!DictionaryHasChatId)
                        {
                            Conversation conversation = values[chatId];
                            TelegramBot.DoConversation(chatId, conversation, update.Message.Text);
                        }
                        else
                        {
                            Conversation conversation = api.Chat.CreateConversation();
                            values.Add(chatId, conversation);
                            TelegramBot.DoConversation(chatId, conversation, update.Message.Text);
                        }
                    }
                    else
                    {
                        Conversation conversation = api.Chat.CreateConversation();
                        values.Add(chatId, conversation);
                        TelegramBot.DoConversation(chatId, conversation, update.Message.Text);
                    }
                }
            }
            
        }
        [HttpGet("init")]
        public string InitChatGPT()
        {
            // ChatGPT initialization
            var chat = api.Chat.CreateConversation();

            /// give instruction as System
            chat.AppendSystemMessage(@"You are a mental health doctor who helps
                anyone solve the problems with mental health. Your name is Alex.
                Be respectful to users, and ask questions only related to mental health.");

            return "Successfully initialized";
        }

        [HttpGet("get-chats")]
        public List<long> GetChats()
        {
            List<long> res = new List<long>();
            if(values != null)
            {
                foreach (var item in values)
                {
                    res.Add(item.Key);
                }
            }

            return res;
        }
    }
}

