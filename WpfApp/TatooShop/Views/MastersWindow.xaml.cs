using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TatooShop.Models;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using TatooShop.ViewModels;
using TatooShop.Services;

namespace TatooShop.Views
{
    /// <summary>
    /// Логика взаимодействия для MastersWindow.xaml
    /// </summary>
    public partial class MastersWindow : Window
    {
        public MastersWindow()
        {
            InitializeComponent();
            if (DataContext is MastersViewModel viewModel)
                viewModel.CloseRequest += (sender, e) => Close();
        }

        private void MasterListScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MasterListScrollViewer.ScrollToVerticalOffset(MasterListScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void Start_Window_Click(object sender, RoutedEventArgs e)
        {
            StartWindow window = new StartWindow();
            window.Show();
        }

        private void Sketch_Window_Click(object sender, RoutedEventArgs e)
        {
            SketchWindow window = new SketchWindow();
            window.Show();
        }
        private void Reservation_Window_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.AccountType != AccountType.User)
            {
                MessageBox.Show("Запись на сеанс доступна только после авторизации.");
                return;
            }

            ReservationWindow window = new ReservationWindow();
            window.Show();
        }
        private void Personal_Window_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.AccountType != AccountType.User)
            {
                MessageBox.Show("Личный кабинет доступен только авторизованным пользователям.");
                return;
            }

            PersonalWindow window = new PersonalWindow();
            window.Show();
        }

        private void Favorite_Window_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.AccountType != AccountType.User)
            {
                MessageBox.Show("Избранное доступно только авторизованным пользователям.");
                return;
            }

            FavoriteWindow window = new FavoriteWindow();
            window.Show();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SessionNavigationService.LogoutToLoginWindow();
        }
    }
}
