using System.Windows;
using TatooShop.Models;
using TatooShop.Services;
using TatooShop.ViewModels;
using TatooShop.Views;

namespace TatooShop.Infrastructure.Commands
{
    class DeleteAdminCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            if (parameter is not Admin admin)
            {
                MessageBox.Show("Не удалось удалить администратора.");
                return;
            }

            if (Manager.CurrentUser?.Id == admin.Id)
            {
                MessageBox.Show("Нельзя удалить аккаунт администратора, под которым вы сейчас вошли.");
                return;
            }

            if (MessageBox.Show(
                    $"Удалить администратора {admin.FullName}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            var db = DataConnection.GetInstance();
            db.Accounts.Remove(admin);
            db.SaveChanges();

            var window = AdminWindow.Instance;
            if (window?.DataContext is AdminViewModel viewModel)
            {
                viewModel.SearchAdmin();
            }
        }
    }
}
