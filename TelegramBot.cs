using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

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

        public static async Task<string> DoConversation(long chatId, string userMessage, ParseMode mode)
        {
            string holdOnText = DotNetEnv.Env.GetString("HOLD_ON_MESSAGE_RU");
            var message = await client.SendTextMessageAsync( chatId: chatId, text: holdOnText, parseMode: mode);

            await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.Typing); // Отправляем "typing" состояние
            await Task.Delay(3000);

            await client.EditMessageTextAsync(chatId: chatId, messageId: message.MessageId, text: userMessage, parseMode: mode);
            return "address should be here";
        }

        public static async Task<EvaluationResult> GetEvaluationResult (string address)
        {
            string apiUrl = "https://akma-aml.azurewebsites.net/tron/check_address/" + address;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        EvaluationResult result = JsonConvert.DeserializeObject<EvaluationResult>(jsonResponse);
                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
