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
    class DeleteMasterCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            if (parameter is Master master)
            {
                if (MessageBox.Show("Удалить мастера?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                var db = DataConnection.GetInstance();
                db.Reservations.RemoveAll(item => item.Master?.Id == master.Id);
                db.Feedbacks.RemoveAll(item => item.Master?.Id == master.Id);
                db.Masters.Remove(master);
                db.SaveChanges();
                var window = AdminWindow.Instance;
                if (window != null)
                {
                    var viewModel = window.DataContext as AdminViewModel;
                    viewModel?.SearchMaster();
                    viewModel?.SearchReservation();
                }

            }
            else
            {
                MessageBox.Show("Не удалось удалить мастера.");
            }
        }
    }
}
