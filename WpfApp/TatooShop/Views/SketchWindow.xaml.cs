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
using TatooShop.ViewModels;
using TatooShop.Models;
using TatooShop.Services;

namespace TatooShop.Views
{
    /// <summary>
    /// Логика взаимодействия для SketchWindow.xaml
    /// </summary>
    public partial class SketchWindow : Window
    {
        public static SketchWindow Instance { get; private set; }
        public SketchWindow()
        {
            InitializeComponent();
            if (DataContext is SketchViewModel viewModel)
                viewModel.CloseRequest += (sender, e) => Close();
            Instance = this;
            Closed += (_, _) => Instance = null;
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

        private void Start_Window_Click(object sender, RoutedEventArgs e)
        {
            StartWindow window = new StartWindow();
            window.Show();
        }

        private void Master_Window_Click(object sender, RoutedEventArgs e)
        {
            MastersWindow window = new MastersWindow();
            window.Show();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SessionNavigationService.LogoutToLoginWindow();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
