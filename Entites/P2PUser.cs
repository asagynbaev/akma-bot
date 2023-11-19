using System.ComponentModel.DataAnnotations;

namespace MindMate.Entities
{
    // Модель для записи пациентов
    public class P2PUser
    {
        [Key]
        public int Id { get; set; }

        public string? Username { get; set; }

        public long TelegramUserId { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Language { get; set; }

        public int Checks { get; set; } = 3;

        public DateTime? CreatedAt { get; set; }

        public string? PhoneNumber { get; set; }

        public bool BlockedByUser { get; set; } = false;
    }
}