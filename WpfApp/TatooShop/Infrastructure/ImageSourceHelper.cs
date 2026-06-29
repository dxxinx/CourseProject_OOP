using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TatooShop.Infrastructure
{
    public static class ImageSourceHelper
    {
        public static ImageSource? ToImageSource(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            using var stream = new MemoryStream(bytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}
