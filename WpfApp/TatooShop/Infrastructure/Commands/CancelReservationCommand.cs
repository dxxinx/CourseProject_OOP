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
    public class CancelReservationCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            return parameter is Reservation reservation &&
                   reservation.Status != ReservationStatus.Visited;
        }

        public override void Execute(object parameter)
        {
            if (parameter is Reservation reservation)
            {
                if (reservation.Status == ReservationStatus.Visited)
                {
                    MessageBox.Show("Нельзя отменить запись со статусом \"Посещена\".");
                    return;
                }

                if (MessageBox.Show("Отменить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                var db = DataConnection.GetInstance();
                db.Reservations.Remove(reservation);
                db.SaveChanges();

                NotificationService.AddNotification(
                    reservation.User,
                    "Запись отменена",
                    $"Извините, ваш сеанс {reservation.Date:dd MMMM yyyy} {Reservation.GetHour(reservation.Time)} по техническим причинам отменен.\nВы можете записаться на другой сеанс.");

                var window = AdminWindow.Instance;
                if (window != null)
                {
                    (window.DataContext as AdminViewModel)?.SearchReservation();
                }
                MessageBox.Show("Запись отменена. Пользователь получит внутреннее уведомление при входе.");
            }
            else
            {
                MessageBox.Show("Не удалось отменить запись.");
            }
        }
    }
}
