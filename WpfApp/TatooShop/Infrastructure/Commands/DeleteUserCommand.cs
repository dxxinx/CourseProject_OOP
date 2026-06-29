using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TatooShop.Models;
using TatooShop.Services;
using TatooShop.ViewModels;
using TatooShop.Views;

namespace TatooShop.Infrastructure.Commands
{
    class DeleteUserCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            if (parameter is Admin)
            {
                MessageBox.Show("Вы не можете удалить администратора");
                return;
            }

            if (parameter is User user)
            {
                if (MessageBox.Show("Вы действительно хотите заблокировать пользователя?", "Подтверждение действия", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var db = DataConnection.GetInstance();
                    db.Reservations.RemoveAll(item => item.User?.Id == user.Id);
                    db.Feedbacks.RemoveAll(item => item.User?.Id == user.Id);
                    db.Favourites.RemoveAll(item => item.User?.Id == user.Id);
                    db.SupportRequests.RemoveAll(item => item.User?.Id == user.Id);
                    db.InternalNotifications.RemoveAll(item => item.User?.Id == user.Id);
                    db.Accounts.Remove(user);
                    db.SaveChanges();

                    var window = AdminWindow.Instance;
                    if (window != null)
                    {
                        var viewModel = window.DataContext as AdminViewModel;
                        viewModel?.SearchUser();
                        viewModel?.SearchReservation();
                        viewModel?.SearchSupportRequests();
                    }
                }
            }
            else
            {
                MessageBox.Show("Не удалось удалить пользователя.");
            }
        }
    }
}
