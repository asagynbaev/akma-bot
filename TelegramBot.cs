using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MindMate
{
	public class TelegramBot
	{
        private static TelegramBotClient client { get; set; }

        public static TelegramBotClient GetTelegramBot()
        {
            string bot_key = DotNetEnv.Env.GetString("TG_BOT");

            client = new TelegramBotClient(bot_key);
            return client;
        }

        public static async Task<Message> SendMessage(long chatId, string userMessage, ParseMode mode) => 
            await client.SendTextMessageAsync( chatId: chatId, text: userMessage, parseMode: mode);

        public static async Task UpdateMessage(long chatId, Message message, string userMessage, ParseMode mode) =>
            await client.EditMessageTextAsync(chatId: chatId, messageId: message.MessageId, text: userMessage, parseMode: mode);

        public static async Task DeleteMessage(long chatId, Message message) =>
            await client.DeleteMessageAsync(chatId: chatId, messageId: message.MessageId);

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
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
    }
}
