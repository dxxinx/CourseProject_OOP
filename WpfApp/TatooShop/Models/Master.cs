using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using TatooShop.Infrastructure;

namespace TatooShop.Models
{
    public class Master
    {
        [Key]
        public int Id { get; set; }

        public byte[] Image { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string MiddleName { get; set; }

        public TatooTypes Type { get; set; }
        public int Experience { get; set; }

        public string FullName => $"{Surname} {Name} {MiddleName}";
        public string ShortFullName => $"{Name} {Surname}";
        public string FullExperience => $"Стаж: {Experience} лет";
        public string FullType => $"Стиль: {Type.GetTitle()}";
        public string Specialization => Type.GetTitle();
        public ImageSource? ImageSource => ImageSourceHelper.ToImageSource(Image);

        public Master()
        {
            Id = -1;
            Image = [];
            Surname = string.Empty;
            Name = string.Empty;
            MiddleName = string.Empty;
        }

        public Master(int id, byte[] image, string surname, string name, string middleName, TatooTypes type, int experience)
        {
            Id = id;
            Image = image;
            Surname = surname;
            Name = name;
            MiddleName = middleName;
            Type = type;
            Experience = experience;
        }

        public string ForSearch() => $"{Surname} {Name} {MiddleName} {Type.GetTitle()}".ToLower();

        public override string ToString() => FullName;
    }
}
