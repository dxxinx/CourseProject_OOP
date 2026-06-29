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
using System.Configuration;
using System.Data.Entity;
using TatooShop.ViewModels;
using TatooShop.Views;

namespace TatooShop.View
{
    /// <summary>
    /// Логика взаимодействия для LogWindow.xaml
    /// </summary>
    /// 
    public partial class LogWindow : Window
    {

        public LogWindow()
        {
            InitializeComponent();
            if (DataContext is LogViewModel viewModel)
                viewModel.CloseRequest += (sender, e) => Close();

        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Create_Account(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                RegistrationWindow reg = new RegistrationWindow();
                reg.ShowDialog();
            }
        }

        private void MinimizeWindow_Button_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Close_Button_Click(object sender, RoutedEventArgs e) => Close();
    }
}
