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
    class ReservMasterCommand : Command
    {
        public override bool CanExecute(object parameter) => Manager.AccountType == AccountType.User;

        public override void Execute(object parameter)
        {
            if (parameter is Master product)
            {
                if (Manager.AccountType != AccountType.User)
                {
                    MessageBox.Show("Запись на сеанс доступна только авторизованным пользователям.");
                    return;
                }

                ReservationViewModel.SelectedMaster = product;
                ReservationWindow window = new ReservationWindow();
                window.Show();
            }
            else
            {
                MessageBox.Show("Не удалось открыть форму записи.");
            }
        }
    }
}
