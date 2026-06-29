using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TatooShop.Infrastructure.Commands;
using TatooShop.Models;
using TatooShop.Services;
using TatooShop.Views;

namespace TatooShop.ViewModels
{
    public class ReservationViewModel : ViewModel
    {
        public static Master SelectedMaster { get; set; }
        public List<Master> MastersList => DataConnection.GetInstance().Masters.ToList();

        public List<string> UserTimeList => Enum.GetValues(typeof(UserTime))
                                          .Cast<UserTime>()
                                          .Select(Reservation.GetHour)
                                          .ToList();
        private List<string> _availableTimeList = new();
        public List<string> AvailableTimeList
        {
            get => _availableTimeList;
            set => SetProperty(ref _availableTimeList, value);
        }
        private Master _master;

        public Master Master
        {
            get { return _master; }
            set
            {
                if (SetProperty(ref _master, value))
                    RefreshAvailableTimeList();
            }
        }
        private string _selectedTime;

        public string SelectedTime
        {
            get { return _selectedTime; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    SetProperty(ref _selectedTime, value);
                    return;
                }

                if (UserTimeList.Contains(value))
                {
                    SetProperty(ref _selectedTime, value);
                }
                else
                {
                    MessageBox.Show("Упс, такого времени не существует");
                }
            }
        }
        private DateTime _date;

        public DateTime UserDate
        {
            get { return _date; }
            set
            {
                if (value.Date >= DateTime.Today)
                {
                    if (SetProperty(ref _date, value.Date))
                        RefreshAvailableTimeList();
                }
                else
                {
                    MessageBox.Show("Упс, даты не существует");
                }
            }
        }
        public int selectedIndex { get; set; } = -1;
        public ICommand ReservationCommand { get; set; }

        private static bool CanReservationCommandExecute(object p) => true;

        private void OnReservationCommandExecuted(object p) => SaveReservation();

        private void SaveReservation()
        {
            if (Manager.CurrentUser is not User currentUser)
            {
                MessageBox.Show("Запись на сеанс доступна только после авторизации.");
                return;
            }

            if (Master != null && !string.IsNullOrEmpty(SelectedTime) && IsSelectedReservationInFuture())
            {
                UserTime userTime = Reservation.ParseHour(SelectedTime);
                var db = DataConnection.GetInstance();
                bool isDuplicateExists = db.Reservations.Any(r =>
                r.Master != null &&
                r.Master.Id == Master.Id &&
                r.Date.Date == UserDate.Date &&
                r.Time == userTime &&
                r.Status != ReservationStatus.Cancelled

            );
                if (isDuplicateExists)
                {
                    MessageBox.Show("Извините, данный сеанс уже занят(");
                }
                else
                {
                    db.Reservations.Add(new Reservation(currentUser, Master, UserDate, Reservation.ParseHour(SelectedTime))
                    {
                        Status = ReservationStatus.Pending
                    });
                    db.SaveChanges();
                    if (PersonalWindow.Instance?.DataContext is PersonalViewModel personalViewModel)
                        personalViewModel.SearchReservation();
                    MessageBox.Show("Заявка на запись создана. Ожидайте подтверждения администратора.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
            }

        }

        private void RefreshAvailableTimeList()
        {
            if (Master == null)
            {
                AvailableTimeList = GetFutureTimeList();
                return;
            }

            var busyTimes = DataConnection.GetReservations()
                .Where(r => r.Master?.Id == Master.Id)
                .Where(r => r.Date.Date == UserDate.Date)
                .Where(r => r.Status != ReservationStatus.Cancelled)
                .Select(r => Reservation.GetHour(r.Time))
                .ToHashSet();

            AvailableTimeList = GetFutureTimeList()
                .Where(time => !busyTimes.Contains(time))
                .ToList();

            if (!AvailableTimeList.Contains(SelectedTime))
                SelectedTime = AvailableTimeList.FirstOrDefault();
        }

        private List<string> GetFutureTimeList()
        {
            return UserTimeList
                .Where(time => UserDate.Date > DateTime.Today ||
                               UserDate.Date.AddHours((int)Reservation.ParseHour(time)) > DateTime.Now)
                .ToList();
        }

        private bool IsSelectedReservationInFuture()
        {
            return UserDate.Date.AddHours((int)Reservation.ParseHour(SelectedTime)) > DateTime.Now;
        }

        public ReservationViewModel() {
            UserDate = DateTime.Today.AddDays(1);

            if (SelectedMaster != null) {
               selectedIndex = MastersList.FindIndex(m => m.Id == SelectedMaster.Id);
               Master = MastersList.FirstOrDefault(m => m.Id == SelectedMaster.Id);
            }
            else
            {
                Master = MastersList.FirstOrDefault();
            }

            RefreshAvailableTimeList();
            SelectedTime = AvailableTimeList.FirstOrDefault();
            ReservationCommand = new RelayCommand(OnReservationCommandExecuted, CanReservationCommandExecute);
        }

    }
}
