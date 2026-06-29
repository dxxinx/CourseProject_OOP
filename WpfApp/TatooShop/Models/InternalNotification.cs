using System;
using System.ComponentModel.DataAnnotations;

namespace TatooShop.Models
{
    public class InternalNotification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public InternalNotification()
        {
            Id = -1;
            Title = string.Empty;
            Message = string.Empty;
            CreatedAt = DateTime.Now;
        }

        public InternalNotification(User user, string title, string message)
            : this()
        {
            User = user;
            Title = title;
            Message = message;
        }
    }
}
