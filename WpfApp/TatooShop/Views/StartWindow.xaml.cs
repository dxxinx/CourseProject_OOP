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
using TatooShop.Services;
using TatooShop.ViewModels;

namespace TatooShop.Views
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            if (DataContext is StartViewModel viewModel)
                viewModel.CloseRequest += (sender, e) => Close();
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
