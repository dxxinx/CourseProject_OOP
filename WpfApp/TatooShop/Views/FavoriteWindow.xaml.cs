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
    /// Логика взаимодействия для FavoriteWindow.xaml
    /// </summary>
    public partial class FavoriteWindow : Window
    {
        public static FavoriteWindow Instance { get; private set; }
        public FavoriteWindow()
        {
            InitializeComponent();

            if (Manager.AccountType != AccountType.User)
            {
                MessageBox.Show("Избранное доступно только авторизованным пользователям.");
                Close();
                return;
            }

            Instance = this;
            Closed += (_, _) => Instance = null;
        }
    }
}
