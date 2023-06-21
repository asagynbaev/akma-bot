using System.ComponentModel.DataAnnotations;

namespace MindMate.Entities
{
    // Модель для записи диалогов
    public class ErrorLogs
    {
        public ErrorLogs(string? message, string? innerMessage, string? causedInMethod)
        {
            Id = Guid.NewGuid();
            Message = message;
            InnerMessage = innerMessage;
            CausedInMethod = causedInMethod;
            Timestamp = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }

        public string? Message { get; set; }

        public string? InnerMessage { get; set; }

        public string? CausedInMethod { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}