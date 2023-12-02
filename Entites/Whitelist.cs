using System.ComponentModel.DataAnnotations;

namespace MindMate.Entities
{
    // Модель для записи черных листов
    public class Whitelist
    {
        [Key]
        public Guid Id { get; set; }

        public string? Address { get; set; }

        public string? AccountName { get; set; }

        public DateTime createdAt { get; set; }
    }
}