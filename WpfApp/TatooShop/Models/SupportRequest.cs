using System;
using System.ComponentModel.DataAnnotations;

namespace TatooShop.Models
{
    public class SupportRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public string AdminReply { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public bool IsProcessed { get; set; }

        public SupportRequest()
        {
            Id = -1;
            CreatedAt = DateTime.Now;
            Subject = string.Empty;
            Message = string.Empty;
        }

        public SupportRequest(User user, string subject, string message)
            : this()
        {
            User = user;
            Subject = subject;
            Message = message;
        }
    }
}
