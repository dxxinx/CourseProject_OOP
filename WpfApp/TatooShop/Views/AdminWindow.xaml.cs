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
using TatooShop.Services;

namespace TatooShop.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public static AdminWindow Instance { get; private set; }
        public AdminWindow()
        {
            InitializeComponent();
            Instance = this;
            Closed += (_, _) => Instance = null;
            UpdateWindowTitle();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SessionNavigationService.LogoutToLoginWindow();
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AdminTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            if (AdminTabControl?.SelectedItem is TabItem selectedTab && selectedTab.Header is string header && !string.IsNullOrWhiteSpace(header))
            {
                Title = header;
            }
        }

        private void DataGrid_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
