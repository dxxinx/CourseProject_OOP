using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatooShop.Models
{
    public class Favourite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Sketch Sketch { get; set; }

        public Favourite() => Id = -1;

        public Favourite(User user, Sketch sketch)
        {
            Id = -1;
            User = user;
            Sketch = sketch;
        }
    }
}
