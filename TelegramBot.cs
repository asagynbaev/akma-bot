using System;
using Telegram.Bot;

namespace MindMate
{
	public class TelegramBot
	{
        private static TelegramBotClient client { get; set; }

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
    }
}

