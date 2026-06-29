using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatooShop.Models;
using TatooShop.Views;
using TatooShop.Services;
using System.Windows;
using TatooShop.ViewModels;

namespace TatooShop.Infrastructure.Commands
{
    public class FavoriteCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            if (parameter is Sketch product)
            {
                var db = DataConnection.GetInstance();
                var currentUser = Manager.CurrentUser as User;

                if (currentUser == null)
                {
                    MessageBox.Show("Добавлять эскизы в избранное могут только авторизованные пользователи.");
                    return;
                }

                var existingRecord = db.Favourites.FirstOrDefault(f => f.User?.Id == currentUser.Id && f.Sketch?.Id == product.Id);

                if (existingRecord != null)
                {
                    db.Favourites.Remove(existingRecord);
                    db.SaveChanges();
                }

                else
                {
                    db.Favourites.Add(new Favourite(currentUser, product));
                    db.SaveChanges();
                }
                var window = SketchWindow.Instance;
                if (window != null)
                {
                    (window.DataContext as SketchViewModel)?.SearchSketch();
                }
                var favoriteWindow = FavoriteWindow.Instance;
                if (favoriteWindow != null)
                {
                    (favoriteWindow.DataContext as FavoriteViewModel)?.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Не удалось изменить избранное.");
            }
        }
    }
}
