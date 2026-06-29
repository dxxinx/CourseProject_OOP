using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Media;
using TatooShop.Infrastructure;
using TatooShop.Services;

namespace TatooShop.Models
{
    public class Sketch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TatooTypes Type { get; set; }

        [Required]
        public TattooPlacement Placement { get; set; }

        [Required]
        public TattooSize Size { get; set; }

        [Required]
        public TattooComplexity Complexity { get; set; }

        public int EstimatedHours { get; set; }

        public byte[] Image { get; set; }

        public Master? Master { get; set; }

        public Sketch()
        {
            Id = -1;
            Image = [];
            Placement = TattooPlacement.None;
            Size = TattooSize.None;
            Complexity = TattooComplexity.None;
        }

        public Sketch(byte[] image, TatooTypes type, Master? master = null)
            : this(image, type, TattooPlacement.None, TattooSize.None, 1, TattooComplexity.None, master)
        {
        }

        public Sketch(
            byte[] image,
            TatooTypes type,
            TattooPlacement placement,
            TattooSize size,
            int estimatedHours,
            TattooComplexity complexity,
            Master? master = null)
        {
            Id = -1;
            Type = type;
            Placement = placement;
            Size = size;
            EstimatedHours = estimatedHours;
            Complexity = complexity;
            Image = image;
            Master = master;
        }

        public string AuthorName => Master?.FullName ?? "Автор не указан";
        public string StyleLabel => Type.GetTitle();
        public string PlacementLabel => Placement.GetTitle();
        public string SizeLabel => Size.GetTitle();
        public string ComplexityLabel => Complexity.GetTitle();
        public string EstimatedTimeLabel => EstimatedHours <= 0 ? "Не указано" : $"{EstimatedHours} ч";
        public ImageSource? ImageSource => ImageSourceHelper.ToImageSource(Image);

        [NotMapped]
        public bool IsFavorite
        {
            get
            {
                var currentUser = Manager.CurrentUser as User;
                return currentUser != null &&
                    DataConnection.GetFavourites().Any(f => f.User?.Id == currentUser.Id && f.Sketch?.Id == Id);
            }
        }
    }

    public enum TatooTypes
    {
        Abstraction,
        BlackWork,
        Minimalism,
        Handpook,
        None
    }

    public enum TattooPlacement
    {
        Arm,
        Leg,
        Thigh,
        Back,
        Chest,
        Neck,
        Shoulder,
        Forearm,
        Wrist,
        Ankle,
        None
    }

    public enum TattooSize
    {
        Small,
        Medium,
        Large,
        Sleeve,
        None
    }

    public enum TattooComplexity
    {
        Simple,
        Medium,
        Complex,
        None
    }

    public enum SketchTimeRange
    {
        UpToTwoHours,
        ThreeToFiveHours,
        SixToEightHours,
        MoreThanEightHours,
        None
    }

    public static class SketchMetadataExtensions
    {
        public static string GetTitle(this TatooTypes type) => type switch
        {
            TatooTypes.Abstraction => "Абстракция",
            TatooTypes.BlackWork => "Блэкворк",
            TatooTypes.Minimalism => "Минимализм",
            TatooTypes.Handpook => "Лайнворк",
            _ => "Все стили"
        };

        public static string GetTitle(this TattooPlacement placement) => placement switch
        {
            TattooPlacement.Arm => "Рука",
            TattooPlacement.Leg => "Нога",
            TattooPlacement.Thigh => "Бедро",
            TattooPlacement.Back => "Спина",
            TattooPlacement.Chest => "Грудь",
            TattooPlacement.Neck => "Шея",
            TattooPlacement.Shoulder => "Плечо",
            TattooPlacement.Forearm => "Предплечье",
            TattooPlacement.Wrist => "Запястье",
            TattooPlacement.Ankle => "Щиколотка",
            _ => "Не указано"
        };

        public static string GetTitle(this TattooSize size) => size switch
        {
            TattooSize.Small => "Маленький",
            TattooSize.Medium => "Средний",
            TattooSize.Large => "Большой",
            TattooSize.Sleeve => "Рукав",
            _ => "Не указано"
        };

        public static string GetTitle(this TattooComplexity complexity) => complexity switch
        {
            TattooComplexity.Simple => "Простой",
            TattooComplexity.Medium => "Средний",
            TattooComplexity.Complex => "Сложный",
            _ => "Не указано"
        };

        public static string GetTitle(this SketchTimeRange range) => range switch
        {
            SketchTimeRange.UpToTwoHours => "До 2 часов",
            SketchTimeRange.ThreeToFiveHours => "3-5 часов",
            SketchTimeRange.SixToEightHours => "6-8 часов",
            SketchTimeRange.MoreThanEightHours => "Больше 8 часов",
            _ => "Любое время"
        };
    }
}
