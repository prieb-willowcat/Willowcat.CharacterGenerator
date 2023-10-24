using Prism.Commands;
using Prism.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Willowcat.CharacterGenerator.EntityFramework.Repository;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.UI.Data;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.CharacterGenerator.UI.ViewModel.Factory;
using Willowcat.Common.UI.ViewModels;
using Willowcat.Common.Utilities;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ChartViewModelFactory _chartViewModelFactory;
        private readonly ChartService _chartService;
        private readonly ICharacterSerializer _characterSerializer;
        private readonly IEventAggregator _eventAggregator;

        private CharacterDetailsViewModel _characterDetailsViewModel;
        private ChartHistoryViewModel _chartHistoryViewModel;
        private ChartViewModel _selectedChart;

        public MainViewModel(
            ChartViewModelFactory factory,
            ChartService chartService,
            IEventAggregator eventAggregator,
            ICharacterSerializer characterSerializer,
            ChartListViewModel chartListViewModel,
            ChartHistoryViewModel chartViewModel)
        {
            _characterSerializer = characterSerializer ?? throw new ArgumentNullException(nameof(characterSerializer));
            _chartViewModelFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _chartService = chartService ?? throw new ArgumentNullException(nameof(chartService));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            CharacterDetailsViewModel = new CharacterDetailsViewModel(_eventAggregator, _characterSerializer);
            ChartListViewModel = chartListViewModel ?? throw new ArgumentNullException(nameof(chartListViewModel));
            ChartHistoryViewModel = chartViewModel ?? throw new ArgumentNullException(nameof(chartViewModel));

            _eventAggregator.GetEvent<ChartSelectedEvent>().Subscribe(OnChartSelectedExecute);

            ReloadChartsCommand = new DelegateCommand(OnReloadChartsExecute);
        }

        public CharacterDetailsViewModel CharacterDetailsViewModel
        {
            get => _characterDetailsViewModel;
            private set
            {
                _characterDetailsViewModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }
        public CharacterFileOptions CharacterFileOptions { get; private set; } = new CharacterFileOptions();
        public ChartListViewModel ChartListViewModel { get; private set; }
        public ChartHistoryViewModel ChartHistoryViewModel
        {
            get => _chartHistoryViewModel; 
            set
            {
                _chartHistoryViewModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand ReloadChartsCommand { get; private set; }
        public ChartViewModel SelectedChart
        {
            get => _selectedChart;
            set
            {
                _selectedChart = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get
            {
                if (CharacterDetailsViewModel == null) return "Character Generator";
                string ChangeIndicator = CharacterDetailsViewModel.HasUnsavedChanges ? "*" : string.Empty;
                return $"Character Generator: {CharacterDetailsViewModel?.CurrentFilePath}{ChangeIndicator}";
            }
        }
        public UserInterfaceSettings UserInterfaceSettings { get; private set; } = new UserInterfaceSettings();

        public async Task LoadDataAsync()
        {
            await ChartListViewModel.LoadChartsAsync();

            await LoadFromFileAsync(CharacterFileOptions.LastOpenedFile);

            string lastOpenedChartKey = CharacterFileOptions.LastOpenedChart;
            if (!string.IsNullOrEmpty(lastOpenedChartKey))
            {
                _eventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(lastOpenedChartKey));
            }
        }

        public async Task LoadFromFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                CharacterModel characterModel = await Task.Run(() =>
                {
                    return _characterSerializer.DeserializeFromFile(_chartService, filePath);
                });
                if (characterModel != null)
                {
                    CharacterDetailsViewModel = new CharacterDetailsViewModel(_eventAggregator, _characterSerializer, characterModel)
                    {
                        CurrentFilePath = filePath
                    };
                    CharacterFileOptions.LastOpenedFile = filePath;
                }
            }
        }

        public void LoadNewCharacter()
        {
            CharacterDetailsViewModel = new CharacterDetailsViewModel(_eventAggregator, _characterSerializer)
            {
                CurrentFilePath = string.Empty
            };
        }

        private void OnChartSelectedExecute(ChartSelectedEventArgs args)
        {
            string lastOpenedChartKey = CharacterFileOptions.LastOpenedChart;
            if (args.ChartKey.EqualsIgnoreCase(lastOpenedChartKey))
            {
                CharacterFileOptions.LastOpenedChart = args.ChartKey;
            }

            SelectedChart = _chartViewModelFactory.CreateViewModelFromKey(args.ChartKey);
            SelectedChart?.Initialize(args.Range);
        }

        public async void OnReloadChartsExecute()
        {
            string selectedChart = SelectedChart?.Key;
            await ChartListViewModel.LoadChartsAsync();
            _eventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(selectedChart));
        }

        public async Task SaveCharacterAsync(string filePath)
        {
            await CharacterDetailsViewModel.SaveCharacterDetailsAsync(filePath);
            CharacterFileOptions.LastOpenedFile = filePath;
            OnPropertyChanged(nameof(Title));
        }
    }
}
