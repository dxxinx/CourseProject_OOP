using System;
using System.Drawing;

using TatooShop.Properties;
using TatooShop.Services;

namespace TatooShop.Infrastructure
{
    internal class ImageProvider
    {
        private static readonly ImageConverter converter = new ImageConverter();

        public static byte[] GetDefault() => (byte[])converter.ConvertTo(Resources.empty, typeof(byte[]));

        public static (bool status, byte[] result) ImageToByte(string img)
        {
            try
            {
                return (true, (byte[])converter.ConvertTo(new Bitmap(img), typeof(byte[])));
            }
            catch (Exception ex)
            {
                return (false, GetDefault());
            }
        }
    }
}
