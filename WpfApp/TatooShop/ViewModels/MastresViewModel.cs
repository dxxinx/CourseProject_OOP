using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TatooShop.Infrastructure.Commands;
using TatooShop.Models;
using TatooShop.Services;
using TatooShop.Views;

namespace TatooShop.ViewModels
{
    public class MastersViewModel : ViewModel, ICloseable
    {
        private readonly DataConnection _db = DataConnection.GetInstance();
        private List<Feedback> _userFeedback = new();
        private List<Master> _masterDataGrid = new();
        private List<Sketch> _portfolioSketches = new();
        private Master? _selectedMaster;
        private int _userRating = 5;
        private string _userComment = string.Empty;
        private string _searchText = string.Empty;
        private string _emptySearchMessage = string.Empty;

        public event EventHandler CloseRequest;

        public User User => Manager.CurrentUser as User;

        public List<Feedback> UserFeedback
        {
            get => _userFeedback;
            set => SetProperty(ref _userFeedback, value);
        }

        public List<Sketch> PortfolioSketches
        {
            get => _portfolioSketches;
            set => SetProperty(ref _portfolioSketches, value);
        }

        public Master? SelectedMaster
        {
            get => _selectedMaster;
            set
            {
                if (SetProperty(ref _selectedMaster, value))
                {
                    OnPropertyChanged(nameof(HasSelectedMaster));
                    OnPropertyChanged(nameof(CanReviewSelectedMaster));
                    CommandManager.InvalidateRequerySuggested();
                    SearchFeedback();
                    RefreshPortfolio();
                }
            }
        }

        public bool HasSelectedMaster => SelectedMaster != null;
        public bool HasNoMasters => MasterDataGrid.Count == 0;
        public bool CanReviewSelectedMaster => User != null && HasVisitedSelectedMaster();

        public List<Master> MasterDataGrid
        {
            get => _masterDataGrid;
            set
            {
                if (SetProperty(ref _masterDataGrid, value))
                {
                    EmptySearchMessage = value.Count == 0 ? "Ничего не найдено" : string.Empty;
                    OnPropertyChanged(nameof(HasNoMasters));
                }
            }
        }

        public string EmptySearchMessage
        {
            get => _emptySearchMessage;
            set => SetProperty(ref _emptySearchMessage, value);
        }

        public int UserRating
        {
            get => _userRating;
            set => SetProperty(ref _userRating, Math.Clamp(value, 1, 5));
        }

        public string UserComment
        {
            get => _userComment;
            set => SetProperty(ref _userComment, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    Search();
            }
        }

        public ICommand SketchCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand ReviewCommand { get; }
        public ICommand OpenPortfolioPreviewCommand { get; }

        public MastersViewModel()
        {
            SearchMaster();
            SelectedMaster = MasterDataGrid.FirstOrDefault();

            SketchCommand = new RelayCommand(_ => OpenSketchWindow(), _ => true);
            StartCommand = new RelayCommand(_ => OpenStartWindow(), _ => true);
            ReviewCommand = new RelayCommand(_ => Review(), _ => CanReviewSelectedMaster);
            OpenPortfolioPreviewCommand = new RelayCommand(OpenPortfolioPreview, parameter => parameter is Sketch);
        }

        public void SearchFeedback()
        {
            if (SelectedMaster == null)
            {
                UserFeedback = new List<Feedback>();
                return;
            }

            UserFeedback = DataConnection.GetFeedbacks()
                .Where(item => item.Master?.Id == SelectedMaster.Id)
                .OrderByDescending(item => item.CreatedAt)
                .ToList();
        }

        private void SearchMaster()
        {
            MasterDataGrid = DataConnection.GetMasters();
        }

        private void RefreshPortfolio()
        {
            PortfolioSketches = SelectedMaster == null
                ? new List<Sketch>()
                : DataConnection.GetSketches()
                    .Where(sketch => sketch.Master?.Id == SelectedMaster.Id)
                    .ToList();
        }

        private void OpenPortfolioPreview(object? parameter)
        {
            if (parameter is not Sketch sketch)
                return;

            var previewWindow = new ImagePreviewWindow(sketch.ImageSource, $"{sketch.AuthorName} • {sketch.StyleLabel}")
            {
                Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(window => window.IsActive)
            };
            previewWindow.ShowDialog();
        }

        private void OpenSketchWindow()
        {
            Window window = new SketchWindow();
            window.Show();
            Application.Current.MainWindow = window;
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OpenStartWindow()
        {
            Window window = new StartWindow();
            window.Show();
            Application.Current.MainWindow = window;
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void Review()
        {
            if (User == null)
            {
                MessageBox.Show("Оставлять отзывы могут только авторизованные пользователи.");
                return;
            }

            if (SelectedMaster == null)
            {
                MessageBox.Show("Сначала выберите мастера.");
                return;
            }

            if (!HasVisitedSelectedMaster())
            {
                MessageBox.Show("Оставить отзыв можно только мастеру, у которого вы уже были на сеансе.");
                return;
            }

            if (string.IsNullOrWhiteSpace(UserComment))
            {
                MessageBox.Show("Введите текст отзыва.");
                return;
            }

            _db.Feedbacks.Add(new Feedback(User, SelectedMaster, UserComment.Trim(), UserRating));
            _db.SaveChanges();
            UserComment = string.Empty;
            UserRating = 5;
            SearchFeedback();
            MessageBox.Show("Отзыв сохранен.");
        }

        private void Search()
        {
            MasterDataGrid = string.IsNullOrWhiteSpace(SearchText)
                ? DataConnection.GetMasters()
                : DataConnection.SearchMasters(SearchText);

            if (SelectedMaster != null && MasterDataGrid.All(master => master.Id != SelectedMaster.Id))
                SelectedMaster = MasterDataGrid.FirstOrDefault();
        }

        private bool HasVisitedSelectedMaster()
        {
            if (User == null || SelectedMaster == null)
                return false;

            return DataConnection.GetReservations().Any(reservation =>
                reservation.User?.Id == User.Id &&
                reservation.Master?.Id == SelectedMaster.Id &&
                reservation.Status == ReservationStatus.Visited);
        }
    }
}
