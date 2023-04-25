using System;
using Telegram.Bot;

namespace MindMate
{
	public class TelegramBot
	{
        private static TelegramBotClient client { get; set; }

        public static TelegramBotClient GetTelegramBot()
        {
            if (client != null)
            {
                return client;
            }
            client = new TelegramBotClient("6001984916:AAEGRpus9lLtFyAUnthmsECTK4wiRmB8qmY");
            return client;
        }
    }
}

