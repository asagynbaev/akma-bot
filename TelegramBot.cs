using System;
using OpenAI_API.Chat;
using Telegram.Bot;

namespace MindMate
{
	public class TelegramBot
	{
        private static TelegramBotClient? client { get; set; }

        public static TelegramBotClient GetTelegramBot()
        {
            string bot_key = DotNetEnv.Env.GetString("TG_BOT");

            if (client != null)
            {
                return client;
            }
            client = new TelegramBotClient(bot_key);
            return client;
        }

        public static async Task<string> DoConversation(long chatId, Conversation conversation, string userMessage)
        {
            Conversation chat = conversation;

            chat.AppendUserInput(userMessage);
            string response = await chat.GetResponseFromChatbotAsync();
            await client.SendTextMessageAsync(chatId, response);

            return response;
        }
    }
}

