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
    public class SketchViewModel : ViewModel, ICloseable
    {
        private readonly List<Sketch> _allSketches = new();
        private List<Sketch> _searchResult = new();
        private TatooTypesFilterItem? _selectedTypeFilter;
        private MasterFilterItem? _selectedMasterFilter;
        private EnumFilterItem<TattooPlacement>? _selectedPlacementFilter;
        private EnumFilterItem<TattooSize>? _selectedSizeFilter;
        private EnumFilterItem<SketchTimeRange>? _selectedTimeFilter;
        private EnumFilterItem<TattooComplexity>? _selectedComplexityFilter;
        private string _emptyStateMessage = string.Empty;

        public event EventHandler CloseRequest;

        public List<Sketch> SearchResult
        {
            get => _searchResult;
            set
            {
                if (SetProperty(ref _searchResult, value))
                {
                    EmptyStateMessage = value.Count == 0 ? "Ничего не найдено" : string.Empty;
                    OnPropertyChanged(nameof(HasNoResults));
                }
            }
        }

        public List<TatooTypesFilterItem> TatooTypesList { get; }
        public List<MasterFilterItem> MasterFilterList { get; }
        public List<EnumFilterItem<TattooPlacement>> PlacementFilterList { get; }
        public List<EnumFilterItem<TattooSize>> SizeFilterList { get; }
        public List<EnumFilterItem<SketchTimeRange>> TimeFilterList { get; }
        public List<EnumFilterItem<TattooComplexity>> ComplexityFilterList { get; }

        public TatooTypesFilterItem? SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set
            {
                if (SetProperty(ref _selectedTypeFilter, value))
                    ApplyFilters();
            }
        }

        public MasterFilterItem? SelectedMasterFilter
        {
            get => _selectedMasterFilter;
            set
            {
                if (SetProperty(ref _selectedMasterFilter, value))
                    ApplyFilters();
            }
        }

        public EnumFilterItem<TattooPlacement>? SelectedPlacementFilter
        {
            get => _selectedPlacementFilter;
            set
            {
                if (SetProperty(ref _selectedPlacementFilter, value))
                    ApplyFilters();
            }
        }

        public EnumFilterItem<TattooSize>? SelectedSizeFilter
        {
            get => _selectedSizeFilter;
            set
            {
                if (SetProperty(ref _selectedSizeFilter, value))
                    ApplyFilters();
            }
        }

        public EnumFilterItem<SketchTimeRange>? SelectedTimeFilter
        {
            get => _selectedTimeFilter;
            set
            {
                if (SetProperty(ref _selectedTimeFilter, value))
                    ApplyFilters();
            }
        }

        public EnumFilterItem<TattooComplexity>? SelectedComplexityFilter
        {
            get => _selectedComplexityFilter;
            set
            {
                if (SetProperty(ref _selectedComplexityFilter, value))
                    ApplyFilters();
            }
        }

        public string EmptyStateMessage
        {
            get => _emptyStateMessage;
            set => SetProperty(ref _emptyStateMessage, value);
        }

        public bool HasNoResults => SearchResult.Count == 0;

        public ICommand MasterCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand OpenPreviewCommand { get; }

        public SketchViewModel()
        {
            _allSketches = DataConnection.GetSketches();
            TatooTypesList = new List<TatooTypesFilterItem>
            {
                new("Все стили", TatooTypes.None)
            };
            TatooTypesList.AddRange(Enum.GetValues(typeof(TatooTypes))
                .Cast<TatooTypes>()
                .Where(type => type != TatooTypes.None)
                .Select(type => new TatooTypesFilterItem(type.GetTitle(), type)));

            MasterFilterList = new List<MasterFilterItem>
            {
                new("Все мастера", null)
            };
            MasterFilterList.AddRange(DataConnection.GetMasters().Select(master => new MasterFilterItem(master.FullName, master)));

            PlacementFilterList = BuildFilterList("Любая локализация", TattooPlacement.None);
            SizeFilterList = BuildFilterList("Любой размер", TattooSize.None);
            TimeFilterList = BuildFilterList("Любое время", SketchTimeRange.None);
            ComplexityFilterList = BuildFilterList("Любая сложность", TattooComplexity.None);

            SelectedTypeFilter = TatooTypesList.First();
            SelectedMasterFilter = MasterFilterList.First();
            SelectedPlacementFilter = PlacementFilterList.First();
            SelectedSizeFilter = SizeFilterList.First();
            SelectedTimeFilter = TimeFilterList.First();
            SelectedComplexityFilter = ComplexityFilterList.First();

            StartCommand = new RelayCommand(_ => OpenStartWindow(), _ => true);
            MasterCommand = new RelayCommand(_ => OpenMasterWindow(), _ => true);
            OpenPreviewCommand = new RelayCommand(OpenPreview, parameter => parameter is Sketch);

            ApplyFilters();
        }

        public void SearchSketch()
        {
            _allSketches.Clear();
            _allSketches.AddRange(DataConnection.GetSketches());
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<Sketch> filtered = _allSketches;

            if (SelectedTypeFilter?.Value is TatooTypes type && type != TatooTypes.None)
                filtered = filtered.Where(sketch => sketch.Type == type);

            if (SelectedMasterFilter?.Value != null)
                filtered = filtered.Where(sketch => sketch.Master?.Id == SelectedMasterFilter.Value.Id);

            if (SelectedPlacementFilter?.Value is TattooPlacement placement && placement != TattooPlacement.None)
                filtered = filtered.Where(sketch => sketch.Placement == placement);

            if (SelectedSizeFilter?.Value is TattooSize size && size != TattooSize.None)
                filtered = filtered.Where(sketch => sketch.Size == size);

            if (SelectedTimeFilter?.Value is SketchTimeRange timeRange && timeRange != SketchTimeRange.None)
                filtered = filtered.Where(sketch => MatchesTimeRange(sketch.EstimatedHours, timeRange));

            if (SelectedComplexityFilter?.Value is TattooComplexity complexity && complexity != TattooComplexity.None)
                filtered = filtered.Where(sketch => sketch.Complexity == complexity);

            SearchResult = filtered.ToList();
        }

        private static List<EnumFilterItem<T>> BuildFilterList<T>(string allTitle, T emptyValue)
            where T : struct, Enum
        {
            var items = new List<EnumFilterItem<T>>
            {
                new(allTitle, emptyValue)
            };

            items.AddRange(Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(value => !EqualityComparer<T>.Default.Equals(value, emptyValue))
                .Select(value => new EnumFilterItem<T>(GetTitle(value), value)));

            return items;
        }

        private static string GetTitle<T>(T value)
            where T : struct, Enum
        {
            return value switch
            {
                TattooPlacement placement => placement.GetTitle(),
                TattooSize size => size.GetTitle(),
                TattooComplexity complexity => complexity.GetTitle(),
                SketchTimeRange range => range.GetTitle(),
                _ => value.ToString()
            };
        }

        private static bool MatchesTimeRange(int estimatedHours, SketchTimeRange range) => range switch
        {
            SketchTimeRange.UpToTwoHours => estimatedHours <= 2,
            SketchTimeRange.ThreeToFiveHours => estimatedHours >= 3 && estimatedHours <= 5,
            SketchTimeRange.SixToEightHours => estimatedHours >= 6 && estimatedHours <= 8,
            SketchTimeRange.MoreThanEightHours => estimatedHours > 8,
            _ => true
        };

        private void OpenPreview(object? parameter)
        {
            if (parameter is not Sketch sketch)
                return;

            var previewWindow = new ImagePreviewWindow(sketch.ImageSource, $"{sketch.AuthorName} • {sketch.StyleLabel}")
            {
                Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(window => window.IsActive)
            };
            previewWindow.ShowDialog();
        }

        private void OpenMasterWindow()
        {
            var window = new MastersWindow();
            window.Show();
            Application.Current.MainWindow = window;
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OpenStartWindow()
        {
            var window = new StartWindow();
            window.Show();
            Application.Current.MainWindow = window;
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }

    public class TatooTypesFilterItem
    {
        public TatooTypesFilterItem(string title, TatooTypes value)
        {
            Title = title;
            Value = value;
        }

        public string Title { get; }
        public TatooTypes Value { get; }

        public override string ToString() => Title;
    }

    public class MasterFilterItem
    {
        public MasterFilterItem(string title, Master? value)
        {
            Title = title;
            Value = value;
        }

        public string Title { get; }
        public Master? Value { get; }

        public override string ToString() => Title;
    }

    public class EnumFilterItem<T>
        where T : struct, Enum
    {
        public EnumFilterItem(string title, T value)
        {
            Title = title;
            Value = value;
        }

        public string Title { get; }
        public T Value { get; }

        public override string ToString() => Title;
    }
}
