using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using TatooShop.Services;
using TatooShop.Infrastructure.Commands;
using TatooShop.Views;


namespace TatooShop.ViewModels
{
    public class LogViewModel : ViewModel, ICloseable
    {
        public event EventHandler CloseRequest;

        private string _login = string.Empty;

        public string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        public ICommand LoginCommand { get; set; }

        private static bool CanLoginCommandExecute(object p) => Manager.CurrentUser == null;

        private void OnLoginCommandExecuted(object p) => ProcessLogin(p);


        public LogViewModel()
        {
            LoginCommand = new RelayCommand(OnLoginCommandExecuted, CanLoginCommandExecute);
        }

        private void ProcessLogin(object p)
        {
            if (string.IsNullOrWhiteSpace(Login))
            {
                MessageBox.Show("Введите логин, email или телефон.");
                return;
            }

            string password = (p as PasswordBox).Password;
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            try
            {
                if (Manager.Login(Login, password))
                {
                    Window window;
                    if (Manager.AccountType == Models.AccountType.User)
                        window = new StartWindow();
                    else
                        window = new AdminWindow();

                    window.Show();
                    Application.Current.MainWindow = window;

                    if (Manager.AccountType == Models.AccountType.User)
                    {
                        NotificationService.ShowUnreadNotificationsForCurrentUser();
                    }

                    CloseRequest?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть окно после авторизации.\n{ex.Message}", "Ошибка");
            }
        }
    }
}
