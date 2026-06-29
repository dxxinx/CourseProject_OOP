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
    class DeleteSketchCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            if (parameter is Sketch sketch)
            {
                if (MessageBox.Show("Удалить эскиз?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                var db = DataConnection.GetInstance();
                db.Favourites.RemoveAll(item => item.Sketch?.Id == sketch.Id);
                db.Sketch.Remove(sketch);
                db.SaveChanges();
                var window = AdminWindow.Instance;
                if (window != null)
                {
                    (window.DataContext as AdminViewModel)?.SearchSketch();
                }
       
            }
            else
            {
                MessageBox.Show("Не удалось удалить эскиз.");
            }
        }
    }
}
