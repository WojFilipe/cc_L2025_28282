using System.ComponentModel.DataAnnotations;

namespace E_testament.Data.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Nadawca wiadomości
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        // Odbiorca wiadomości – null = wiadomość publiczna (czat grupowy)
        public int? ReceiverId { get; set; }
        public User? Receiver { get; set; }
    }
}
