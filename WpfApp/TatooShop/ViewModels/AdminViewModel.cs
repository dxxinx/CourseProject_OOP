using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TatooShop.Infrastructure;
using TatooShop.Infrastructure.Commands;
using TatooShop.Models;
using TatooShop.Services;
using TatooShop.Views;

namespace TatooShop.ViewModels
{
    public class AdminViewModel : ViewModel
    {
        private readonly DataConnection _db = DataConnection.GetInstance();

        private string _query = string.Empty;
        private string _adminQuery = string.Empty;
        private string _masterSearchText = string.Empty;
        private TatooTypes _filterType = TatooTypes.None;
        private EnumFilterItem<TattooPlacement>? _filterPlacement;
        private EnumFilterItem<TattooSize>? _filterSize;
        private EnumFilterItem<SketchTimeRange>? _filterTime;
        private EnumFilterItem<TattooComplexity>? _filterComplexity;
        private TatooTypes _selectedSketchType = TatooTypes.None;
        private TattooPlacement _selectedSketchPlacement = TattooPlacement.None;
        private TattooSize _selectedSketchSize = TattooSize.None;
        private TattooComplexity _selectedSketchComplexity = TattooComplexity.None;
        private TatooTypes _selectedMasterType = TatooTypes.None;
        private byte[] _image;
        private byte[] _imageMaster;
        private string _estimatedHours = string.Empty;
        private string _surname = string.Empty;
        private string _name = string.Empty;
        private string _middleName = string.Empty;
        private string _experience = string.Empty;
        private string _supportReply = string.Empty;
        private string _adminLogin = string.Empty;
        private string _adminPassword = string.Empty;
        private string _adminEmail = string.Empty;
        private string _adminPhone = string.Empty;
        private string _adminSurname = string.Empty;
        private string _adminName = string.Empty;
        private string _adminMiddleName = string.Empty;
        private Sketch? _selectedSketch;
        private Reservation _selectedReservation;
        private SupportRequest? _selectedSupportRequest;
        private Master? _selectedSketchMaster;

        public string Query
        {
            get => _query;
            set
            {
                if (SetProperty(ref _query, value))
                    SearchUser();
            }
        }

        public string MasterSearchText
        {
            get => _masterSearchText;
            set
            {
                if (SetProperty(ref _masterSearchText, value))
                    SearchMaster();
            }
        }

        public string AdminQuery
        {
            get => _adminQuery;
            set
            {
                if (SetProperty(ref _adminQuery, value))
                    SearchAdmin();
            }
        }

        public List<User> UserDataGrid { get; set; } = new();
        public List<Admin> AdminDataGrid { get; set; } = new();
        public List<Sketch> SketchDataGrid { get; set; } = new();
        public List<Master> MasterDataGrid { get; set; } = new();
        public List<Reservation> ReservationDataGrid { get; set; } = new();
        public List<SupportRequest> SupportRequestDataGrid { get; set; } = new();

        public List<TatooTypesFilterItem> TatooTypesList { get; } = BuildStyleList("Выберите стиль", false);
        public List<TatooTypesFilterItem> TatooTypesFilterList { get; } = BuildStyleList("Все стили", true);
        public List<EnumFilterItem<TattooPlacement>> PlacementList { get; } = BuildPlacementList("Выберите локализацию", TattooPlacement.None);
        public List<EnumFilterItem<TattooSize>> SizeList { get; } = BuildSizeList("Выберите размер", TattooSize.None);
        public List<EnumFilterItem<TattooComplexity>> ComplexityList { get; } = BuildComplexityList("Выберите сложность", TattooComplexity.None);
        public List<EnumFilterItem<TattooPlacement>> PlacementFilterList { get; } = BuildPlacementList("Любая локализация", TattooPlacement.None);
        public List<EnumFilterItem<TattooSize>> SizeFilterList { get; } = BuildSizeList("Любой размер", TattooSize.None);
        public List<EnumFilterItem<SketchTimeRange>> TimeFilterList { get; } = BuildTimeList("Любое время", SketchTimeRange.None);
        public List<EnumFilterItem<TattooComplexity>> ComplexityFilterList { get; } = BuildComplexityList("Любая сложность", TattooComplexity.None);
        public List<Master> SketchMasterList => DataConnection.GetMasters();

        public byte[] Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public TatooTypes Type
        {
            get => _selectedSketchType;
            set => SetProperty(ref _selectedSketchType, value);
        }

        public TattooPlacement Placement
        {
            get => _selectedSketchPlacement;
            set => SetProperty(ref _selectedSketchPlacement, value);
        }

        public TattooSize Size
        {
            get => _selectedSketchSize;
            set => SetProperty(ref _selectedSketchSize, value);
        }

        public TattooComplexity Complexity
        {
            get => _selectedSketchComplexity;
            set => SetProperty(ref _selectedSketchComplexity, value);
        }

        public string EstimatedHours
        {
            get => _estimatedHours;
            set => SetProperty(ref _estimatedHours, value);
        }

        public Master? SelectedSketchMaster
        {
            get => _selectedSketchMaster;
            set => SetProperty(ref _selectedSketchMaster, value);
        }

        public string Surname
        {
            get => _surname;
            set => SetProperty(ref _surname, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string MiddleName
        {
            get => _middleName;
            set => SetProperty(ref _middleName, value);
        }

        public string Experience
        {
            get => _experience;
            set => SetProperty(ref _experience, value);
        }

        public byte[] ImageMaster
        {
            get => _imageMaster;
            set => SetProperty(ref _imageMaster, value);
        }

        public TatooTypes TypeMaster
        {
            get => _selectedMasterType;
            set => SetProperty(ref _selectedMasterType, value);
        }

        public TatooTypes FilterType
        {
            get => _filterType;
            set
            {
                if (SetProperty(ref _filterType, value))
                    SearchSketch();
            }
        }

        public Sketch? SelectedSketch
        {
            get => _selectedSketch;
            set
            {
                if (SetProperty(ref _selectedSketch, value))
                {
                    LoadSelectedSketch();
                    OnPropertyChanged(nameof(SketchFormTitle));
                    OnPropertyChanged(nameof(SketchSaveButtonText));
                }
            }
        }

        public string SketchFormTitle => SelectedSketch == null ? "Добавить эскиз" : "Редактировать эскиз";
        public string SketchSaveButtonText => SelectedSketch == null ? "Сохранить эскиз" : "Обновить эскиз";

        public EnumFilterItem<TattooPlacement>? FilterPlacement
        {
            get => _filterPlacement;
            set
            {
                if (SetProperty(ref _filterPlacement, value))
                    SearchSketch();
            }
        }

        public EnumFilterItem<TattooSize>? FilterSize
        {
            get => _filterSize;
            set
            {
                if (SetProperty(ref _filterSize, value))
                    SearchSketch();
            }
        }

        public EnumFilterItem<SketchTimeRange>? FilterTime
        {
            get => _filterTime;
            set
            {
                if (SetProperty(ref _filterTime, value))
                    SearchSketch();
            }
        }

        public EnumFilterItem<TattooComplexity>? FilterComplexity
        {
            get => _filterComplexity;
            set
            {
                if (SetProperty(ref _filterComplexity, value))
                    SearchSketch();
            }
        }

        public Reservation SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                if (SetProperty(ref _selectedReservation, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public SupportRequest? SelectedSupportRequest
        {
            get => _selectedSupportRequest;
            set
            {
                if (SetProperty(ref _selectedSupportRequest, value))
                {
                    SupportReply = value?.AdminReply ?? string.Empty;
                    OnPropertyChanged(nameof(CanReplySupportRequest));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string SupportReply
        {
            get => _supportReply;
            set => SetProperty(ref _supportReply, value);
        }

        public bool CanReplySupportRequest => SelectedSupportRequest != null && !SelectedSupportRequest.IsProcessed;

        public string AdminLogin
        {
            get => _adminLogin;
            set => SetProperty(ref _adminLogin, value);
        }

        public string AdminPassword
        {
            get => _adminPassword;
            set => SetProperty(ref _adminPassword, value);
        }

        public string AdminEmail
        {
            get => _adminEmail;
            set => SetProperty(ref _adminEmail, value);
        }

        public string AdminPhone
        {
            get => _adminPhone;
            set => SetProperty(ref _adminPhone, value);
        }

        public string AdminSurname
        {
            get => _adminSurname;
            set => SetProperty(ref _adminSurname, value);
        }

        public string AdminName
        {
            get => _adminName;
            set => SetProperty(ref _adminName, value);
        }

        public string AdminMiddleName
        {
            get => _adminMiddleName;
            set => SetProperty(ref _adminMiddleName, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand SearchAdminCommand { get; }
        public ICommand SearchMasterCommand { get; }
        public ICommand ImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ClearSketchFormCommand { get; }
        public ICommand ImageMasterCommand { get; }
        public ICommand SaveMasterCommand { get; }
        public ICommand ConfirmReservationCommand { get; }
        public ICommand ProcessSupportRequestCommand { get; }
        public ICommand SaveAdminCommand { get; }

        public AdminViewModel()
        {
            Image = ImageProvider.GetDefault();
            ImageMaster = ImageProvider.GetDefault();
            FilterPlacement = PlacementFilterList.First();
            FilterSize = SizeFilterList.First();
            FilterTime = TimeFilterList.First();
            FilterComplexity = ComplexityFilterList.First();

            SearchCommand = new RelayCommand(_ => SearchUser(), _ => true);
            SearchAdminCommand = new RelayCommand(_ => SearchAdmin(), _ => true);
            SearchMasterCommand = new RelayCommand(_ => SearchMaster(), _ => true);
            ImageCommand = new RelayCommand(_ => LoadImg(), _ => true);
            SaveCommand = new RelayCommand(_ => SaveSketch(), _ => true);
            ClearSketchFormCommand = new RelayCommand(_ => ClearSketchForm(), _ => true);
            ImageMasterCommand = new RelayCommand(_ => LoadImgMaster(), _ => true);
            SaveMasterCommand = new RelayCommand(_ => SaveMaster(), _ => true);
            ConfirmReservationCommand = new RelayCommand(_ => ConfirmReservation(), _ => CanConfirmSelectedReservation());
            ProcessSupportRequestCommand = new RelayCommand(_ => ProcessSupportRequest(), _ => CanReplySupportRequest);
            SaveAdminCommand = new RelayCommand(_ => SaveAdmin(), _ => true);

            SearchUser();
            SearchAdmin();
            SearchSketch();
            SearchMaster();
            SearchReservation();
            SearchSupportRequests();
        }

        public void SearchUser()
        {
            UserDataGrid = DataConnection.SearchUsers(Query);
            OnPropertyChanged(nameof(UserDataGrid));
        }

        public void SearchAdmin()
        {
            AdminDataGrid = DataConnection.SearchAdmins(AdminQuery);
            OnPropertyChanged(nameof(AdminDataGrid));
        }

        public void SearchSketch()
        {
            IEnumerable<Sketch> sketches = DataConnection.GetSketches();

            if (FilterType != TatooTypes.None)
                sketches = sketches.Where(sketch => sketch.Type == FilterType);

            if (FilterPlacement?.Value is TattooPlacement placement && placement != TattooPlacement.None)
                sketches = sketches.Where(sketch => sketch.Placement == placement);

            if (FilterSize?.Value is TattooSize size && size != TattooSize.None)
                sketches = sketches.Where(sketch => sketch.Size == size);

            if (FilterTime?.Value is SketchTimeRange timeRange && timeRange != SketchTimeRange.None)
                sketches = sketches.Where(sketch => MatchesTimeRange(sketch.EstimatedHours, timeRange));

            if (FilterComplexity?.Value is TattooComplexity complexity && complexity != TattooComplexity.None)
                sketches = sketches.Where(sketch => sketch.Complexity == complexity);

            SketchDataGrid = sketches.ToList();
            OnPropertyChanged(nameof(SketchDataGrid));
        }

        public void SearchMaster()
        {
            var masters = string.IsNullOrWhiteSpace(MasterSearchText)
                ? DataConnection.GetMasters()
                : DataConnection.SearchMasters(MasterSearchText);

            MasterDataGrid = masters;
            OnPropertyChanged(nameof(SketchMasterList));
            if (SelectedSketchMaster == null || SketchMasterList.All(master => master.Id != SelectedSketchMaster.Id))
                SelectedSketchMaster = SketchMasterList.FirstOrDefault();
            OnPropertyChanged(nameof(MasterDataGrid));
        }

        public void SearchReservation()
        {
            ReservationDataGrid = DataConnection.GetReservations()
                .OrderBy(reservation => reservation.Status)
                .ThenBy(reservation => reservation.Date)
                .ThenBy(reservation => reservation.Time)
                .ToList();
            OnPropertyChanged(nameof(ReservationDataGrid));
        }

        public void SearchSupportRequests()
        {
            SupportRequestDataGrid = DataConnection.GetSupportRequests()
                .OrderBy(request => request.IsProcessed)
                .ThenByDescending(request => request.CreatedAt)
                .ToList();
            OnPropertyChanged(nameof(SupportRequestDataGrid));
        }

        private void SaveSketch()
        {
            if (Image == null || Image.Length == 0 || Type == TatooTypes.None)
            {
                MessageBox.Show("Пожалуйста, выберите стиль и изображение эскиза.");
                return;
            }

            if (Placement == TattooPlacement.None || Size == TattooSize.None || Complexity == TattooComplexity.None)
            {
                MessageBox.Show("Выберите локализацию, размер и сложность эскиза.");
                return;
            }

            if (!int.TryParse(EstimatedHours, out var estimatedHours) || estimatedHours <= 0)
            {
                MessageBox.Show("Укажите примерное время выполнения в часах.");
                return;
            }

            if (SelectedSketchMaster == null)
            {
                MessageBox.Show("Выберите мастера, который создал эскиз.");
                return;
            }

            var sketchToUpdate = SelectedSketch;
            var isNewSketch = sketchToUpdate == null;
            if (sketchToUpdate is null)
            {
                _db.Sketch.Add(new Sketch(Image, Type, Placement, Size, estimatedHours, Complexity, SelectedSketchMaster));
            }
            else
            {
                sketchToUpdate.Image = Image;
                sketchToUpdate.Type = Type;
                sketchToUpdate.Placement = Placement;
                sketchToUpdate.Size = Size;
                sketchToUpdate.EstimatedHours = estimatedHours;
                sketchToUpdate.Complexity = Complexity;
                sketchToUpdate.Master = SelectedSketchMaster;
            }

            _db.SaveChanges();
            ClearSketchForm();
            SearchSketch();
            MessageBox.Show(isNewSketch ? "Эскиз сохранен." : "Эскиз обновлен.");
        }

        private void LoadSelectedSketch()
        {
            if (SelectedSketch == null)
                return;

            Image = SelectedSketch.Image;
            Type = SelectedSketch.Type;
            Placement = SelectedSketch.Placement;
            Size = SelectedSketch.Size;
            EstimatedHours = SelectedSketch.EstimatedHours.ToString();
            Complexity = SelectedSketch.Complexity;
            SelectedSketchMaster = SketchMasterList.FirstOrDefault(master => master.Id == SelectedSketch.Master?.Id)
                ?? SelectedSketch.Master
                ?? SketchMasterList.FirstOrDefault();
        }

        private void ClearSketchForm()
        {
            SelectedSketch = null;
            Image = ImageProvider.GetDefault();
            Type = TatooTypes.None;
            Placement = TattooPlacement.None;
            Size = TattooSize.None;
            EstimatedHours = string.Empty;
            Complexity = TattooComplexity.None;
            SelectedSketchMaster = SketchMasterList.FirstOrDefault();
            OnPropertyChanged(nameof(SketchFormTitle));
            OnPropertyChanged(nameof(SketchSaveButtonText));
        }

        private void SaveMaster()
        {
            if (string.IsNullOrWhiteSpace(Surname) ||
                string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(MiddleName) ||
                TypeMaster == TatooTypes.None ||
                !int.TryParse(Experience, out var experience) ||
                experience < 0 ||
                ImageMaster == null ||
                ImageMaster.Length == 0)
            {
                MessageBox.Show("Пожалуйста, заполните данные мастера корректно.");
                return;
            }

            _db.Masters.Add(new Master(-1, ImageMaster, Surname, Name, MiddleName, TypeMaster, experience));
            _db.SaveChanges();
            ClearMasterForm();
            SearchMaster();
            SelectedSketchMaster ??= SketchMasterList.FirstOrDefault();
            MessageBox.Show("Мастер сохранен.");
        }

        private void ClearMasterForm()
        {
            Surname = string.Empty;
            Name = string.Empty;
            MiddleName = string.Empty;
            Experience = string.Empty;
            TypeMaster = TatooTypes.None;
            ImageMaster = ImageProvider.GetDefault();
        }

        private void SaveAdmin()
        {
            if (string.IsNullOrWhiteSpace(AdminLogin) ||
                string.IsNullOrWhiteSpace(AdminPassword) ||
                string.IsNullOrWhiteSpace(AdminEmail) ||
                string.IsNullOrWhiteSpace(AdminPhone) ||
                string.IsNullOrWhiteSpace(AdminSurname) ||
                string.IsNullOrWhiteSpace(AdminName) ||
                string.IsNullOrWhiteSpace(AdminMiddleName))
            {
                MessageBox.Show("Заполните все поля администратора.");
                return;
            }

            if (!Validator.loginRegex.IsMatch(AdminLogin))
            {
                MessageBox.Show("Логин должен начинаться с латинской буквы и содержать только латинские буквы, цифры, точку или дефис.");
                return;
            }

            if (!Validator.eMailRegex.IsMatch(AdminEmail))
            {
                MessageBox.Show("Введите корректный email администратора.");
                return;
            }

            if (!Validator.phoneRegex.IsMatch(AdminPhone))
            {
                MessageBox.Show("Введите телефон в формате +375XXXXXXXXX или 80XXXXXXXXX.");
                return;
            }

            if (!Validator.nameRegex.IsMatch(AdminSurname) ||
                !Validator.nameRegex.IsMatch(AdminName) ||
                !Validator.nameRegex.IsMatch(AdminMiddleName))
            {
                MessageBox.Show("ФИО должно содержать только буквы, пробелы или дефис.");
                return;
            }

            if (_db.Accounts.Any(account => account.Login == AdminLogin.Trim()))
            {
                MessageBox.Show("Логин уже занят.");
                return;
            }

            if (_db.Accounts.Any(account => account.EMail == AdminEmail.Trim()))
            {
                MessageBox.Show("Почта уже занята.");
                return;
            }

            if (_db.Accounts.Any(account => account.Phone == AdminPhone.Trim()))
            {
                MessageBox.Show("Телефон уже занят.");
                return;
            }

            _db.Accounts.Add(new Admin(
                AdminLogin.Trim(),
                Manager.HashPassword(AdminPassword),
                AdminEmail.Trim(),
                AdminPhone.Trim(),
                AdminSurname.Trim(),
                AdminName.Trim(),
                AdminMiddleName.Trim()));

            _db.SaveChanges();
            ClearAdminForm();
            SearchAdmin();
            MessageBox.Show("Аккаунт администратора создан.");
        }

        private void ClearAdminForm()
        {
            AdminLogin = string.Empty;
            AdminPassword = string.Empty;
            AdminEmail = string.Empty;
            AdminPhone = string.Empty;
            AdminSurname = string.Empty;
            AdminName = string.Empty;
            AdminMiddleName = string.Empty;
        }

        private void ConfirmReservation()
        {
            if (SelectedReservation == null)
                return;

            if (!CanConfirmSelectedReservation())
            {
                MessageBox.Show(SelectedReservation.Status == ReservationStatus.Visited
                    ? "Нельзя подтвердить запись со статусом \"Посещена\"."
                    : "Эту запись нельзя подтвердить повторно.");
                return;
            }

            SelectedReservation.Status = ReservationStatus.Confirmed;
            _db.SaveChanges();
            SearchReservation();

            NotificationService.AddNotification(
                SelectedReservation.User,
                "Запись подтверждена",
                $"Ваша запись на {SelectedReservation.Date:dd.MM.yyyy} в {Reservation.GetHour(SelectedReservation.Time)} подтверждена.");

            MessageBox.Show("Запись подтверждена. Пользователь получит внутреннее уведомление при входе.");
        }

        private bool CanConfirmSelectedReservation()
        {
            return SelectedReservation?.Status == ReservationStatus.Pending;
        }

        private void ProcessSupportRequest()
        {
            if (SelectedSupportRequest == null)
                return;

            if (SelectedSupportRequest.IsProcessed)
            {
                MessageBox.Show("Это обращение уже обработано. Повторный ответ недоступен.");
                return;
            }

            if (string.IsNullOrWhiteSpace(SupportReply))
            {
                MessageBox.Show("Введите ответ для пользователя.");
                return;
            }

            SelectedSupportRequest.AdminReply = SupportReply.Trim();
            SelectedSupportRequest.IsProcessed = true;
            SelectedSupportRequest.ProcessedAt = DateTime.Now;
            _db.SaveChanges();
            SearchSupportRequests();
            OnPropertyChanged(nameof(CanReplySupportRequest));
            CommandManager.InvalidateRequerySuggested();

            NotificationService.AddNotification(
                SelectedSupportRequest.User,
                $"Ответ по обращению: {SelectedSupportRequest.Subject}",
                SelectedSupportRequest.AdminReply);

            if (PersonalWindow.Instance?.DataContext is PersonalViewModel personalViewModel)
                personalViewModel.SearchSupportRequests();

            SelectedSupportRequest = null;
            SupportReply = string.Empty;
            MessageBox.Show("Обращение обработано. Пользователь увидит ответ как внутреннее уведомление.");
        }

        private void LoadImg()
        {
            var dialog = new OpenFileDialog { Filter = "Картинки| *.jpg;*.jpeg;*.png;*.bmp" };
            if (dialog.ShowDialog() != true)
                return;

            var (status, result) = ImageProvider.ImageToByte(dialog.FileName);
            if (status)
                Image = result;
            else
                MessageBox.Show("Не удалось загрузить изображение.");
        }

        private void LoadImgMaster()
        {
            var dialog = new OpenFileDialog { Filter = "Картинки| *.jpg;*.jpeg;*.png;*.bmp" };
            if (dialog.ShowDialog() != true)
                return;

            var (status, result) = ImageProvider.ImageToByte(dialog.FileName);
            if (status)
                ImageMaster = result;
            else
                MessageBox.Show("Не удалось загрузить изображение.");
        }

        private static List<EnumFilterItem<TattooPlacement>> BuildPlacementList(string firstTitle, TattooPlacement firstValue)
        {
            var items = new List<EnumFilterItem<TattooPlacement>> { new(firstTitle, firstValue) };
            items.AddRange(Enum.GetValues(typeof(TattooPlacement))
                .Cast<TattooPlacement>()
                .Where(value => value != firstValue)
                .Select(value => new EnumFilterItem<TattooPlacement>(value.GetTitle(), value)));
            return items;
        }

        private static List<TatooTypesFilterItem> BuildStyleList(string firstTitle, bool includeEmpty)
        {
            var items = new List<TatooTypesFilterItem>();

            if (includeEmpty)
                items.Add(new TatooTypesFilterItem(firstTitle, TatooTypes.None));

            items.AddRange(Enum.GetValues(typeof(TatooTypes))
                .Cast<TatooTypes>()
                .Where(type => type != TatooTypes.None)
                .Select(type => new TatooTypesFilterItem(type.GetTitle(), type)));

            if (!includeEmpty)
                items.Insert(0, new TatooTypesFilterItem(firstTitle, TatooTypes.None));

            return items;
        }

        private static List<EnumFilterItem<TattooSize>> BuildSizeList(string firstTitle, TattooSize firstValue)
        {
            var items = new List<EnumFilterItem<TattooSize>> { new(firstTitle, firstValue) };
            items.AddRange(Enum.GetValues(typeof(TattooSize))
                .Cast<TattooSize>()
                .Where(value => value != firstValue)
                .Select(value => new EnumFilterItem<TattooSize>(value.GetTitle(), value)));
            return items;
        }

        private static List<EnumFilterItem<TattooComplexity>> BuildComplexityList(string firstTitle, TattooComplexity firstValue)
        {
            var items = new List<EnumFilterItem<TattooComplexity>> { new(firstTitle, firstValue) };
            items.AddRange(Enum.GetValues(typeof(TattooComplexity))
                .Cast<TattooComplexity>()
                .Where(value => value != firstValue)
                .Select(value => new EnumFilterItem<TattooComplexity>(value.GetTitle(), value)));
            return items;
        }

        private static List<EnumFilterItem<SketchTimeRange>> BuildTimeList(string firstTitle, SketchTimeRange firstValue)
        {
            var items = new List<EnumFilterItem<SketchTimeRange>> { new(firstTitle, firstValue) };
            items.AddRange(Enum.GetValues(typeof(SketchTimeRange))
                .Cast<SketchTimeRange>()
                .Where(value => value != firstValue)
                .Select(value => new EnumFilterItem<SketchTimeRange>(value.GetTitle(), value)));
            return items;
        }

        private static bool MatchesTimeRange(int estimatedHours, SketchTimeRange range) => range switch
        {
            SketchTimeRange.UpToTwoHours => estimatedHours <= 2,
            SketchTimeRange.ThreeToFiveHours => estimatedHours >= 3 && estimatedHours <= 5,
            SketchTimeRange.SixToEightHours => estimatedHours >= 6 && estimatedHours <= 8,
            SketchTimeRange.MoreThanEightHours => estimatedHours > 8,
            _ => true
        };
    }
}
