using System;
using System.Windows;
using TatooShop.Models;
using TatooShop.Services;

namespace TatooShop.Infrastructure.Commands
{
    public class DeleteCurrentUserCommand : Command
    {
        public override bool CanExecute(object parameter) => Manager.CurrentUser is User;

        public override void Execute(object parameter)
        {
            if (Manager.CurrentUser is not User currentUser)
            {
                MessageBox.Show("Удалить аккаунт можно только после входа пользователя.");
                return;
            }

            if (MessageBox.Show(
                    "Вы действительно хотите удалить свой аккаунт? Все ваши записи, избранное, отзывы и обращения будут удалены без восстановления.",
                    "Удаление аккаунта",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                var db = DataConnection.GetInstance();
                var userId = currentUser.Id;

                db.Reservations.RemoveAll(item => item.User?.Id == userId);
                db.Feedbacks.RemoveAll(item => item.User?.Id == userId);
                db.Favourites.RemoveAll(item => item.User?.Id == userId);
                db.SupportRequests.RemoveAll(item => item.User?.Id == userId);
                db.InternalNotifications.RemoveAll(item => item.User?.Id == userId);
                db.Accounts.RemoveAll(item => item.Id == userId && item.AccountType == AccountType.User);
                db.SaveChanges();

                MessageBox.Show("Аккаунт удалён.");
                SessionNavigationService.LogoutToLoginWindow();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить аккаунт.");
            }
        }
    }
}
