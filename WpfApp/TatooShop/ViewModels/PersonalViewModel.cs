using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TatooShop.Infrastructure.Commands;
using TatooShop.Models;
using TatooShop.Services;

namespace TatooShop.ViewModels
{
    public class PersonalViewModel : ViewModel
    {
        private string _supportSubject = string.Empty;
        private string _supportMessage = string.Empty;

        public User User => Manager.CurrentUser as User;

        public ObservableCollection<Reservation> UserReservation { get; set; } = new();
        public ObservableCollection<SupportRequest> UserSupportRequests { get; set; } = new();
        public ObservableCollection<Favourite> UserFavorite { get; set; } = new();

        public string SupportSubject
        {
            get => _supportSubject;
            set => SetProperty(ref _supportSubject, value);
        }

        public string SupportMessage
        {
            get => _supportMessage;
            set => SetProperty(ref _supportMessage, value);
        }

        public ICommand SendSupportRequestCommand { get; }
        public ICommand DeleteFavoriteCommand { get; }

        public PersonalViewModel()
        {
            SearchReservation();
            SearchSupportRequests();
            SearchFavorites();
            SendSupportRequestCommand = new RelayCommand(_ => SendSupportRequest(), _ => User != null);
            DeleteFavoriteCommand = new RelayCommand(DeleteFavorite, item => item is Favourite);
        }

        public void SearchReservation()
        {
            UserReservation = new ObservableCollection<Reservation>(
                DataConnection.GetReservations()
                    .Where(item => item.User?.Id == Manager.CurrentUser?.Id)
                    .OrderByDescending(item => item.Date)
                    .ThenBy(item => item.Time));
            OnPropertyChanged(nameof(UserReservation));
        }

        public void SearchSupportRequests()
        {
            UserSupportRequests = new ObservableCollection<SupportRequest>(
                DataConnection.GetSupportRequests()
                    .Where(item => item.User?.Id == Manager.CurrentUser?.Id)
                    .OrderByDescending(item => item.CreatedAt));
            OnPropertyChanged(nameof(UserSupportRequests));
        }

        public void SearchFavorites()
        {
            UserFavorite = new ObservableCollection<Favourite>(
                DataConnection.GetFavourites()
                    .Where(item => item.User?.Id == Manager.CurrentUser?.Id));
            OnPropertyChanged(nameof(UserFavorite));
        }

        private void DeleteFavorite(object parameter)
        {
            if (parameter is not Favourite favorite)
                return;

            var db = DataConnection.GetInstance();
            db.Favourites.Remove(favorite);
            db.SaveChanges();

            UserFavorite.Remove(favorite);
        }

        private void SendSupportRequest()
        {
            if (User == null)
            {
                MessageBox.Show("Отправить обращение можно только после авторизации.");
                return;
            }

            if (string.IsNullOrWhiteSpace(SupportSubject) || string.IsNullOrWhiteSpace(SupportMessage))
            {
                MessageBox.Show("Заполните тему и текст обращения.");
                return;
            }

            var db = DataConnection.GetInstance();
            db.SupportRequests.Add(new SupportRequest(User, SupportSubject.Trim(), SupportMessage.Trim()));
            db.SaveChanges();

            SupportSubject = string.Empty;
            SupportMessage = string.Empty;
            SearchSupportRequests();
            MessageBox.Show("Обращение отправлено в техническую поддержку.");
        }
    }
}
