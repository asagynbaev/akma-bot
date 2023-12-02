using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindMate.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MindMate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly DialogContext _context;
        private readonly TelegramBotClient _telegramBotClient;

        public MainController(ILogger<MainController> logger, DialogContext context)
        {
            _logger = logger;
            _context = context;
            _telegramBotClient = TelegramBot.GetTelegramBot();
        }

        private TelegramBotClient bot = TelegramBot.GetTelegramBot();

        [HttpPost("talk")]
        public async Task Post([FromBody] Update update) //Update receiver method
        {
            try
            {
                if (update.Message?.Text != null)
                {
                    // Getting user's telegram user id
                    long chatId = update.Message.Chat.Id;
                    
                    // Check if this is a first interaction
                    if (update.Message.Text == "/start")
                    {
                        await SaveUser(update.Message.Chat.Id, update.Message.Chat.Username, update.Message.Chat.FirstName, update.Message.Chat.LastName, "ru");
                        await TelegramBot.SendMessage(
                            update.Message.Chat.Id,
                            "Привет! 👋 Добро пожаловать в Akma! AML бот для проверки чистоты USDT Tether TRC20 кошелька! \n \n " + 
                            "Мы здесь, чтобы обеспечить вас надежным инструментом для проверки кошелька на соответствие стандартам безопасности и предотвращения отмывания денег. " + 
                            "Просто отправьте номер вашего кошелька, и мы проверим его статус на предмет чистоты. 🤖🔍🔐 \n \n " + 
                            "В качестве приятного бонуса, Akma предоставляет Вам 3 бесплатные проверки:)",
                            ParseMode.Markdown
                        );
                    }
                    else
                    {
                        if(update.Message.Text == "/balance")
                        {
                            P2PUser? user = await _context.Users.SingleOrDefaultAsync(x => x.TelegramUserId == update.Message.Chat.Id);
                            await TelegramBot.SendMessage(chatId, "Количество проверок: " + user?.Checks.ToString(), ParseMode.Html);
                        }
                        else if(update.Message.Text == "/check")
                        {
                            //P2PUser user = await _context.Users.SingleOrDefaultAsync(x => x.TelegramUserId == update.Message.Chat.Id);
                            await TelegramBot.SendMessage(
                                chatId, 
                                "Для того, чтобы проверить адрес USDT (TRC20), скопируйте его в буфер обмена и вставьте его в поле ниже. \n \n" + 
                                "Пример: TG6Udj1YeqXQhr7aSteVf28iWmV1vMtWeA", 
                                ParseMode.Html
                            );
                        }
                        else if(update.Message.Text == "/about")
                        {
                            //P2PUser user = await _context.Users.SingleOrDefaultAsync(x => x.TelegramUserId == update.Message.Chat.Id);
                            await TelegramBot.SendMessage(
                                chatId, 
                                "Akma AML Scanner обеспечивает надежную защиту криптовалютных транзакций в сети <b>Tron(TRC20)</b>, применяя интеллектуальный алгоритм анализа для проверки адресов кошельков. \n \n " + 
                                "Более подробнее о Akma можно прочитать на сайте: <a href=\"https://akma-aml-technologies-inc.gitbook.io/welcome/\">Akma AML Screener</a> \n \n " +
                                "Перед тем, как начать использование бота, пожалуйста, прочтите правила использования: <a href=\"https://akma-aml-technologies-inc.gitbook.io/welcome/pravila-ispolzovaniya\">тут</a> \n \n " +
                                "А также ознакомьтесь с политикой конфиденциальности: <a href=\"https://akma-aml-technologies-inc.gitbook.io/welcome/politika-konfidencialnosti\">тут</a> \n \n ",
                                ParseMode.Html
                            );
                        }
                        else
                        {
                            if (IsValidUsdtTrc20Address(update.Message.Text))
                            {
                                var orderNumber = TelegramBot.GenerateOrderNumber();
                                var message = await TelegramBot.SendMessage(chatId, $"Заказ #{orderNumber} \n \n " + DotNetEnv.Env.GetString("HOLD_ON_MESSAGE_RU"), ParseMode.Html);
                                
                                var blacklistExists = await _context.Blacklists.SingleOrDefaultAsync(x => x.Address == update.Message.Text);
                                if(blacklistExists != null)
                                {
                                    await TelegramBot.UpdateMessage(
                                        chatId,
                                        message,
                                        $"Заказ #{orderNumber} \n \n " +
                                        $"Внимание! адрес <b>{update.Message.Text}</b> находится в санкционном списке OFAC или является подозрительным. \n \n " +
                                        "Не рекомендуется заключать сделки с этим адресом.",
                                        ParseMode.Html
                                    );
                                    return;
                                }

                                var whitelist = await _context.Whitelists.SingleOrDefaultAsync(x => x.Address == update.Message.Text);
                                if(whitelist != null)
                                {
                                    await TelegramBot.UpdateMessage(
                                        chatId,
                                        message,
                                        $"Заказ #{orderNumber}\n\n" +
                                        $"Внимание! Адрес *{update.Message.Text}* является биржевым ({whitelist.AccountName}).\n\n" +
                                        "Рекомендуем вам быть осторожными при совершении сделок с этим адресом. Учитывайте, что клиенты бирж обычно проходят процедуру KYC.",
                                        ParseMode.Html
                                    );
                                    return;
                                }

                                var result = await TelegramBot.GetEvaluationResult(update.Message.Text);
                                
                                if((result != null) && (result.Message != null))
                                {
                                    await TelegramBot.UpdateMessage(
                                        chatId,
                                        message,
                                        $"Внимание! {result.Message}",
                                        ParseMode.Html
                                    );
                                    return;
                                }
                                else if((result != null) && (result.FinalEvaluation != null))
                                {
                                    await TelegramBot.UpdateMessage(
                                        chatId,
                                        message,
                                        $"Номер проверки: #{orderNumber} \n \n " +
                                        $"📈 Степень риска(0-100): {result.FinalEvaluation.FinalEvaluation} \n\n" + 
                                        $"📊 Количество транзакций: {result.FinalEvaluation.Transactions} \n\n" + 
                                        $"⛔️ Находится в санкционном списке OFAC: {(result.FinalEvaluation.Blacklist ? "✅ Да" : "❌ Нет")} \n\n" + 
                                        $"💀 Опасность во версии TronScan: <b>{result.FinalEvaluation.RedTag}</b> \n\n" + 
                                        $"💰 Баланс кошелька: {result.FinalEvaluation.Balance} USDT \n\n" +
                                        $"🕐 Дата первой транзакции: {result.FinalEvaluation.First_Transaction} \n\n" + 
                                        $"🕠 Дата последней транзакции: {result.FinalEvaluation.Last_Transaction}", 
                                        ParseMode.Html
                                    );

                                    if(result.FinalEvaluation.Blacklist == true)
                                    {
                                        Blacklist blacklist = new Blacklist
                                        {
                                            Id = Guid.NewGuid(),
                                            Address = update.Message.Text,
                                            BlacklistType = "OFAC",
                                            OrderNumber = orderNumber,
                                            createdAt = DateTime.UtcNow
                                        };

                                        _context.Blacklists.Add(blacklist);
                                        _context.SaveChanges();
                                    }

                                    if(result.FinalEvaluation.RedTag == "Подозрительный")
                                    {
                                        Blacklist blacklist = new Blacklist
                                        {
                                            Id = Guid.NewGuid(),
                                            Address = update.Message.Text,
                                            BlacklistType = "TronScan",
                                            OrderNumber = orderNumber,
                                            createdAt = DateTime.UtcNow
                                        };

                                        _context.Blacklists.Add(blacklist);
                                        _context.SaveChanges();
                                    }

                                    var dialog = new Dialog
                                    {
                                        Username = update.Message.Chat.Username,
                                        UserMessage = update.Message.Text,
                                        BotResponse =  $"Message: {result.Message} Error: {result.Error} Evaluation: {result.FinalEvaluation.FinalEvaluation}",
                                        Timestamp = DateTime.UtcNow,
                                        OrderNumber = orderNumber,
                                        TelegramUserId = update.Message.Chat.Id.ToString()
                                    };

                                    _context.Dialogs.Add(dialog);
                                    _context.SaveChanges();
                                }  
                                else
                                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                    await TelegramBot.UpdateMessage(
                                        chatId,
                                        message,
                                        $"Упс! Свяжитесь с администратором. @akma_aml_support \n Ошибка: {result.Error}",
                                        ParseMode.Html
                                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                                }
                            }
                            else
                            {
                                await TelegramBot.SendMessage(chatId, "Invalid USDT TRC20 address.", ParseMode.Html);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                _context.Errors.Add(new ErrorLogs(ex.Message, ex.InnerException?.Message, "talk"));
                await _context.SaveChangesAsync();
            }
        }
        
        // [HttpGet("send-notification/{text}")]
        // public async Task SendReminderMessage(string text)
        // {
        //     try
        //     {
        //         List<P2PUser> patients = await _context.Users.ToListAsync();
        //         var message = text;
        //         foreach(var item in patients)
        //         {
        //             if(item.TelegramUserId != 0)
        //             {
        //                 await TelegramBot.DoConversation(item.TelegramUserId, message);
        //             }
        //         }
        //     }
        //     catch(Exception ex)
        //     {
        //         _logger.LogError(ex.Message);
        //         _context.Errors.Add(new ErrorLogs(ex.Message, ex.InnerException.Message, "talk"));
        //         await _context.SaveChangesAsync();
        //     }
        // }

        [HttpGet("send-message-to-users-who-didnt-use/{text}")]
        public async Task SendFirst(string text)
        {
            long tuserid = 0;
            try
            {
                List<P2PUser> patients = await _context.Users.ToListAsync();
                List<Dialog> dialogs = await _context.Dialogs.ToListAsync();

                // Retrieve patients that do not exist in the Dialog table
                IEnumerable<P2PUser> patientsNotInDialog = patients.Where(p => !dialogs.Any(d => d.TelegramUserId == p.TelegramUserId.ToString() && d.TelegramUserId != null));

                var message = text;
                foreach(var item in patients)
                {
                    if(item.TelegramUserId != 0 && !item.BlockedByUser)
                    {
                        tuserid = item.TelegramUserId;
                        await TelegramBot.SendMessage(item.TelegramUserId, message, ParseMode.MarkdownV2);
                    }
                }
            }
            catch(Exception ex)
            {
                if(ex.Message == "Forbidden: bot was blocked by the user")
                {
                    P2PUser? user = await _context.Users.SingleOrDefaultAsync(x => x.TelegramUserId == tuserid);
                    if(user != null)
                    {
                        user.BlockedByUser = true;
                        await _context.SaveChangesAsync();
                    }
                }
                _logger.LogError(ex.Message);
                var error = new ErrorLogs(ex.Message, ex.InnerException?.Message, "send-message-to-users-who-didnt-use");
                _context.Errors.Add(error);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SaveUser(long patientId, string? username, string? firstname, string? lastname, string language)
        {
            P2PUser? ifExists = await _context.Users.SingleOrDefaultAsync(x => x.TelegramUserId == patientId);

            // Check if user exists, if not, put it in database
            if(ifExists == null)
            {
                P2PUser user = new P2PUser()
                {
                    Username = username,
                    TelegramUserId = patientId,
                    Firstname = firstname,
                    Lastname = lastname,
                    CreatedAt = DateTime.UtcNow,
                    Language = language
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Saved: {user.Id}, {user.Username}");
            }
        }

        static bool IsValidUsdtTrc20Address(string address)
        {
            // Adjust the regex pattern based on the actual format of USDT TRC20 addresses
            string pattern = @"^T[a-km-zA-HJ-NP-Z1-9]{33}$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(address);
        }
    }
}
