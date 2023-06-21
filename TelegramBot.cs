using OpenAI_API.Chat;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace MindMate
{
	public class TelegramBot
	{
        private static TelegramBotClient? client { get; set; }

        public static TelegramBotClient GetTelegramBot()
        {
            string bot_key = DotNetEnv.Env.GetString("TG_BOT");

            if (client != null)
                return client;
                
            client = new TelegramBotClient(bot_key);
            return client;
        }

        // Метод для рассылки массовых сообщений
        public static async Task<string> DoConversation(long chatId, string message)
        {
            await client.SendTextMessageAsync(chatId, message);
            return "Ok";
        }

        // Метод для отрпавки сообщения в рамках беседы с чат ботом
        public static async Task<string> DoConversation(long chatId, Conversation conversation, string userMessage, string lang)
        {
            Conversation chat = conversation;

            chat.AppendUserInput(userMessage);
            string holdOnText = "";

            if(lang == "ru")
                holdOnText = DotNetEnv.Env.GetString("HOLD_ON_MESSAGE_RU");
            else
                holdOnText = DotNetEnv.Env.GetString("HOLD_ON_MESSAGE_EN");

            // Отправляем сообщение с информацией о том, что бот обрабатывает запрос
            var message = await client.SendTextMessageAsync(chatId, holdOnText);
            await Task.Delay(1000); // Ждем 1 секунду для имитации обработки

            await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.Typing); // Отправляем "typing" состояние
            string response = await chat.GetResponseFromChatbotAsync();

            // Обновляем сообщение с новым ответом
            await client.EditMessageTextAsync(chatId, message.MessageId, response);

            return response;
        }

        // Метод для отрпавки сообщения в рамках беседы с чат ботом
        public static async Task<string> DoConversation(long chatId, ReplyKeyboardMarkup replyKeyboardMarkup, string userMessage)
        {
            // Отправляем сообщение с информацией о том, что бот обрабатывает запрос
            var message = await client.SendTextMessageAsync(chatId, userMessage, replyMarkup: replyKeyboardMarkup);
            return "Ok";
        }
    }
}
