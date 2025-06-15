using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MessagesEntity
    {
        [Required]
        public Guid MessageID { get; set; } = Guid.Empty;

        [Required]
        public Guid SenderID { get; set; } = Guid.Empty;
        [Required]
        public Guid ReceiverID { get; set; } = Guid.Empty;

        [Required]
        public string MessageContent { get; set; } = string.Empty;

        [Required]
        public DateTime SentOn { get; set; } = DateTime.Now;

        [Required]
        public int MessageStatus { get; set; } = 0;

    }
}
