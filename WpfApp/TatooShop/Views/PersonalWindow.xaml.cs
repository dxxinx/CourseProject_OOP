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

namespace TatooShop.Views
{
    /// <summary>
    /// Логика взаимодействия для PersonalWindow.xaml
    /// </summary>
    public partial class PersonalWindow : Window
    {
        public static PersonalWindow Instance{ get; private set; }   
        public PersonalWindow()
        {
            InitializeComponent();

            if (Manager.AccountType != AccountType.User)
            {
                MessageBox.Show("Личный кабинет доступен только авторизованным пользователям.");
                Close();
                return;
            }

            Instance = this;
            Closed += (_, _) => Instance = null;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SessionNavigationService.LogoutToLoginWindow();
        }
    }
}
