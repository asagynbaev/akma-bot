using System.ComponentModel.DataAnnotations;

namespace MindMate.Entities
{
    // Модель для записи диалогов
    public class Dialog
    {
        [Key]
        public int Id { get; set; }

        public string OrderNumber { get; set; }

        public string? Username { get; set; }

        public string? TelegramUserId { get; set; }

        public string? UserMessage { get; set; }

        public string? BotResponse { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}