using System;
using OpenAI_API.Chat;
using Telegram.Bot;
using Telegram.Bot.Types;

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

            // Отправляем сообщение с информацией о том, что бот обрабатывает запрос
            var message = await client.SendTextMessageAsync(chatId, "Люссид обрабатывает ваш запрос...");
            await Task.Delay(1000); // Ждем 1 секунду для имитации обработки

            await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.Typing); // Отправляем "typing" состояние
            string response = await chat.GetResponseFromChatbotAsync();

            // Обновляем сообщение с новым ответом
            await client.EditMessageTextAsync(chatId, message.MessageId, response);

            return response;
        }
    }
}
