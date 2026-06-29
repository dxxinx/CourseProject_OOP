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
    internal class DeleteFavoriteCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            if (parameter is Favourite product)
            {
                var db = DataConnection.GetInstance();
                db.Favourites.Remove(product);
                db.SaveChanges();
        
                var window = FavoriteWindow.Instance;
                if (window != null)
                {
                    (window.DataContext as FavoriteViewModel)?.Refresh();
                }
                var window2 = SketchWindow.Instance;
                if (window2 != null)
                {
                    (window2.DataContext as SketchViewModel)?.SearchSketch();
                }
            }
            else
            {
                MessageBox.Show("Не удалось удалить эскиз из избранного.");
            }
        }
    }
}
