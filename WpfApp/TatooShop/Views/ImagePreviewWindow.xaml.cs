using System.Windows;
using System.Windows.Media;

namespace TatooShop.Views
{
    public partial class ImagePreviewWindow : Window
    {
        public ImagePreviewWindow(ImageSource? imageSource, string title)
        {
            InitializeComponent();
            PreviewImage.Source = imageSource;
            PreviewTitle.Text = title;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
