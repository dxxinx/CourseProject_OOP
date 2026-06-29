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
    internal class DeleteReservationCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            return parameter is Reservation reservation &&
                   reservation.Status != ReservationStatus.Visited;
        }

        public override void Execute(object parameter)
        {
            if (parameter is Reservation product)
            {
                if (product.Status == ReservationStatus.Visited)
                {
                    MessageBox.Show("Нельзя отменить запись со статусом \"Посещена\".");
                    return;
                }

                if (MessageBox.Show("Отменить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                var db = DataConnection.GetInstance();
                db.Reservations.Remove(product);
                db.SaveChanges();
                var window = PersonalWindow.Instance;
                if (window != null)
                {
                    var viewModel = window.DataContext as PersonalViewModel;
                    viewModel?.SearchReservation();
                }
            }
            else
            {
                MessageBox.Show("Не удалось отменить запись.");
            }
        }
    }
}
