using System.ComponentModel.DataAnnotations;

namespace MindMate.Entities
{
    // Модель для записи черных листов
    public class Blacklist
    {
        [Key]
        public Guid Id { get; set; }

        public string? Address { get; set; }

        public string? BlacklistType { get; set; }

        public string OrderNumber { get; set; }

        public DateTime createdAt { get; set; }
    }
}