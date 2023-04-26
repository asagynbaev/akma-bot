using System;
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
                    var chat = api.Chat.CreateConversation();
                    chat.AppendUserInput($@"You are a mental health doctor who helps
anyone solve the problems with mental health. Your name is {DotNetEnv.Env.GetString("ASSISTANT_NAME")}.
Be respectful to users, and ask questions only related to mental health.");

                    long chatId = update.Message.Chat.Id;
                    await bot.SendTextMessageAsync(chatId, $@"Hello {update.Message.Chat.Username}!
Welcome to AI Mental Health support.My name is {DotNetEnv.Env.GetString("ASSISTANT_NAME")}.
How are you doing? I need more details about you. Tell me what is your name and how old are you.");
                }
                else
                {
                    var conversation = api.Chat.CreateConversation();
                    long chatId = update.Message.Chat.Id;
                    TelegramBot.DoConversation(chatId, conversation, update.Message.Text);
                }
            }
        }

        [HttpGet("init")]
        public string InitChatGPT()
        {
            // ChatGPT initialization
            var chat = api.Chat.CreateConversation();

            chat.AppendUserInput($@"You are a mental health doctor who helps
anyone solve the problems with mental health. Your name is {DotNetEnv.Env.GetString("ASSISTANT_NAME")}.
Be respectful to users, and ask questions only related to mental health.");

            return "Successfully initialized";
        }
    }
}

