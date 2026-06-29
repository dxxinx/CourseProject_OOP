using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TatooShop.Models;
using TatooShop.Services;

namespace TatooShop.ViewModels
{
    internal class FavoriteViewModel : ViewModel
    {
        public User User => Manager.CurrentUser as User;

        private ObservableCollection<Favourite> _userFavorite = new();
        public ObservableCollection<Favourite> UserFavorite
        {
            get => _userFavorite;
            set => SetProperty(ref _userFavorite, value);
        }


        public void Refresh()
        {
            if (Manager.CurrentUser is not User currentUser)
            {
                UserFavorite = new ObservableCollection<Favourite>();
                return;
            }

            UserFavorite = new ObservableCollection<Favourite>(
                DataConnection.GetFavourites().Where(f => f.User?.Id == currentUser.Id));
        }

        public FavoriteViewModel()
        {
            Refresh();
        }
    }
}
