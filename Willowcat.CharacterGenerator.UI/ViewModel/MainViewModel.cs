using Prism.Commands;
using Prism.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.TextRepository;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.UI.Data;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;
using Willowcat.Common.Utilities;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ChartService _ChartService;
        private readonly ICharacterSerializer _CharacterSerializer;
        private readonly IEventAggregator _EventAggregator;

        private CharacterDetailsViewModel _CharacterDetailsViewModel;
        private ChartHistoryViewModel _ChartHistoryViewModel;

        public CharacterDetailsViewModel CharacterDetailsViewModel
        {
            get => _CharacterDetailsViewModel;
            private set
            {
                _CharacterDetailsViewModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }
        public CharacterFileOptions CharacterFileOptions { get; private set; } = new CharacterFileOptions();
        public ChartListViewModel ChartListViewModel { get; private set; }
        public ChartHistoryViewModel ChartHistoryViewModel
        {
            get => _ChartHistoryViewModel; 
            set
            {
                _ChartHistoryViewModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand ReloadChartsCommand { get; private set; }
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

        public MainViewModel(
            ChartService chartService,
            IEventAggregator eventAggregator,
            ICharacterSerializer characterSerializer,
            ChartListViewModel chartListViewModel,
            ChartHistoryViewModel chartViewModel)
        {
            _CharacterSerializer = characterSerializer ?? throw new ArgumentNullException(nameof(_CharacterSerializer));
            _ChartService = chartService ?? throw new ArgumentNullException(nameof(_ChartService));
            _EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(_EventAggregator));

            CharacterDetailsViewModel = new CharacterDetailsViewModel(_EventAggregator, _CharacterSerializer);
            ChartListViewModel = chartListViewModel ?? throw new ArgumentNullException(nameof(chartListViewModel));
            ChartHistoryViewModel = chartViewModel ?? throw new ArgumentNullException(nameof(chartViewModel));

            _EventAggregator.GetEvent<ChartSelectedEvent>().Subscribe(OnChartSelectedExecute);

            ReloadChartsCommand = new DelegateCommand(OnReloadChartsExecute);
        }

        public async Task LoadDataAsync()
        {
            await ChartListViewModel.LoadChartsAsync();

            await LoadFromFileAsync(CharacterFileOptions.LastOpenedFile);

            string lastOpenedChartKey = CharacterFileOptions.LastOpenedChart;
            if (!string.IsNullOrEmpty(lastOpenedChartKey))
            {
                _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(lastOpenedChartKey));
            }
        }

        public async Task LoadFromFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                CharacterModel characterModel = await Task.Run(() =>
                {
                    return _CharacterSerializer.DeserializeFromFile(_ChartService, filePath);
                });
                if (characterModel != null)
                {
                    CharacterDetailsViewModel = new CharacterDetailsViewModel(_EventAggregator, _CharacterSerializer, characterModel)
                    {
                        CurrentFilePath = filePath
                    };
                    CharacterFileOptions.LastOpenedFile = filePath;
                }
            }
        }

        public void LoadNewCharacter()
        {
            CharacterDetailsViewModel = new CharacterDetailsViewModel(_EventAggregator, _CharacterSerializer)
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
        }

        public async void OnReloadChartsExecute()
        {
            // TODO: ChartViewModel.Chart.Key; LoadChartsAsync
            string SelectedChart = ChartHistoryViewModel.SelectedChart?.Key;
            await ChartListViewModel.LoadChartsAsync();
            _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(SelectedChart));
        }

        public async Task SaveCharacterAsync(string filePath)
        {
            await CharacterDetailsViewModel.SaveCharacterDetailsAsync(filePath);
            CharacterFileOptions.LastOpenedFile = filePath;
            OnPropertyChanged(nameof(Title));
        }
    }
}
