using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.UI.Commands;
using Willowcat.CharacterGenerator.UI.Data;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class ChartViewModel : ViewModelBase
    {
        private readonly ChartService _ChartService = null;
        private readonly IEventAggregator _EventAggregator = null;

        private readonly Stack<string> _NextCharts = new Stack<string>();
        private readonly Stack<string> _PreviousCharts = new Stack<string>();

        private bool _IsProcessRunning = false;
        private CharacterDetailsViewModel _CharacterDetailsViewModel;
        private ChartModel _Chart = null;
        private int _SelectedIndex = -1;
        private int? _Result = null;
        private int? _Modifier = null;
        private FlowDocument _ChartDescriptionDocument = null;
        private FlowDocument _SelectedOptionDescriptionDocument = null;
        private ObservableCollection<RegionOption> _RegionOptions = new ObservableCollection<RegionOption>();
        private RegionOption _SelectedRegion = null;

        public bool CanDynamicallyGenerateOptions => _Chart?.CanDynamicallyGenerateOptions ?? false;

        public CharacterDetailsViewModel CharacterDetailsViewModel
        {
            get => _CharacterDetailsViewModel;
            set
            {
                _CharacterDetailsViewModel = value;
                OnPropertyChanged();
            }
        }

        public ChartModel Chart => _Chart;

        public FlowDocument ChartDescriptionDocument
        {
            get => _ChartDescriptionDocument;
            set
            {
                _ChartDescriptionDocument = value;
                OnPropertyChanged();
            }
        }

        public string ChartDice
        {
            get => _Chart?.Dice.ToString() ?? string.Empty;
        }

        public ICommand ChartHyperLinkCommand { get; private set; }

        public string ChartLocationString => string.Join(" > ", _ChartService.GetChartHierarchyPath(_Chart));

        public string ChartName
        {
            get => _Chart?.ChartName ?? "Chart";
        }

        public ObservableCollection<ChartOptionDetailModel> ChartOptions { get; private set; } = new ObservableCollection<ChartOptionDetailModel>();

        public AsyncCommand GenerateOptionsCommand { get; private set; }

        public DelegateCommand GoToNextCommand { get; private set; }

        public DelegateCommand GoToPreviousCommand { get; private set; }

        public bool IsProcessRunning
        {
            get => _IsProcessRunning;
            set
            {
                _IsProcessRunning = value;
                OnPropertyChanged();
            }
        }

        public int? Modifier
        {
            get => _Modifier;
            set
            {
                _Modifier = value;
                OnPropertyChanged();
            }
        }

        public Random Randomizer { get; set; }

        public ObservableCollection<RegionOption> RegionOptions
        {
            get => _RegionOptions;
            set
            {
                _RegionOptions = value;
                OnPropertyChanged();
            }
        }

        public int? Result
        {
            get => _Result;
            set
            {
                _Result = value;
                OnPropertyChanged();
            }
        }

        public ICommand RollCommand { get; private set; }

        public RegionOption SelectedRegion
        {
            get => _SelectedRegion;
            set
            {
                _SelectedRegion = value;
                if (value != null)
                {
                    Properties.Settings.Default.LastRegionSelected = value.Key;
                    Properties.Settings.Default.Save();
                    _Chart.LoadSavedOptions(value.Key);
                }
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                _SelectedIndex = value;
                OnPropertyChanged();
                UpdateChartDetail();
            }
        }

        public FlowDocument SelectedOptionDescriptionDocument
        {
            get => _SelectedOptionDescriptionDocument;
            set
            {
                _SelectedOptionDescriptionDocument = value;
                OnPropertyChanged();
            }
        }

        public bool ShowRegionSelector => _Chart?.ShowRegionSelector ?? false;

        public ICommand UseSelectionCommand { get; private set; }

        public ChartViewModel(ChartService chartService, IEventAggregator eventAggregator)
        {
            _ChartService = chartService ?? throw new ArgumentNullException(nameof(chartService));
            _EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            ChartHyperLinkCommand = new DelegateCommand<string>(OnChartHyperLinkExecute);
            GoToNextCommand = new DelegateCommand(OnGoToNextExecute, OnGoToNextCanExecute);
            GoToPreviousCommand = new DelegateCommand(OnGoToPreviousExecute, OnGoToPreviousCanExecute);
            GenerateOptionsCommand = new AsyncCommand(OnGenerateOptionsExecute, OnGenerateOptionsCanExecute);
            RollCommand = new DelegateCommand(OnRollExecute);
            UseSelectionCommand = new DelegateCommand(OnUseSelectionExecute);

            _EventAggregator.GetEvent<ChartSelectedEvent>().Subscribe(OnChartSelectedEvent);
        }

        private void OnChartHyperLinkExecute(string chartKey)
        {
            _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(chartKey));
        }

        private void OnChartSelectedEvent(ChartSelectedEventArgs args)
        {
            UpdateNavigationHistory(args);

            _Chart = _ChartService.GetChart(args.ChartKey);

            Modifier = null;
            Result = null;
            if (_Chart != null)
            {
                RefreshOptions();
                RefreshChart(args.Range);
            }
            else
            {
                ChartDescriptionDocument = null;
                SelectedOptionDescriptionDocument = null;
            }

            OnPropertyChanged(nameof(ChartDice));
            OnPropertyChanged(nameof(ChartName));
            OnPropertyChanged(nameof(ChartLocationString));
            OnPropertyChanged(nameof(ShowRegionSelector));
            OnPropertyChanged(nameof(CanDynamicallyGenerateOptions));
            GenerateOptionsCommand.RaiseCanExecuteChanged();
        }

        private bool OnGenerateOptionsCanExecute() => _Chart?.CanDynamicallyGenerateOptions ?? false;

        private async Task OnGenerateOptionsExecute()
        {
            try
            {
                if (_Chart.CanDynamicallyGenerateOptions)
                {
                    IsProcessRunning = true;
                    await _Chart.GenerateOptionsAsync(Randomizer, _SelectedRegion?.Key);
                    RefreshChart();
                    OnPropertyChanged(nameof(ChartDice));
                }
            }
            //catch (Exception ex)
            //{
            //    ShowError(ex);
            //}
            finally
            {
                IsProcessRunning = false;
            }
        }

        private bool OnGoToNextCanExecute() => _NextCharts.Any();

        private void OnGoToNextExecute()
        {
            if (_NextCharts.Any())
            {
                if (_Chart != null)
                {
                    _PreviousCharts.Push(_Chart.Key);
                }
                string nextChartKey = _NextCharts.Pop();
                _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(nextChartKey, false));
            }
        }

        private bool OnGoToPreviousCanExecute() => _PreviousCharts.Any();

        private void OnGoToPreviousExecute()
        {
            if (_PreviousCharts.Any())
            {
                if (_Chart != null)
                {
                    _NextCharts.Push(_Chart.Key);
                }
                string previousChartKey = _PreviousCharts.Pop();
                _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(previousChartKey, false));
            }
        }

        private void OnRollExecute()
        {
            if (_Chart != null)
            {
                int result = _Chart.Dice.Roll(Randomizer, Modifier ?? 0);
                Result = result;
                SelectResult(result);
            }
        }

        private async void OnUseSelectionExecute()
        {
            if (CharacterDetailsViewModel != null && SelectedIndex >= 0 && SelectedIndex < ChartOptions.Count)
            {
                Guid selectedId = ChartOptions[SelectedIndex].OptionId;
                var selectedOption = _Chart.GetSelectedOption(selectedId);
                await CharacterDetailsViewModel.AddCharacterDetailAsync(selectedOption);
            }
        }

        private void RefreshChart(int? valueToSelect = null)
        {
            ChartOptions.Clear();
            if (_Chart != null)
            {
                foreach (var option in _Chart.Options.OrderBy(x => x.Range.Start))
                {
                    ChartOptions.Add(new ChartOptionDetailModel(_ChartService, _EventAggregator, option));
                }
                ChartDescriptionDocument = RichTextStringFormatters.AddLocalLinksToChartKeysDocument(_ChartService, _Chart.Notes, ChartHyperLinkCommand);
                if (valueToSelect.HasValue)
                {
                    SelectResult(valueToSelect.Value);
                }
            }
        }

        private void RefreshOptions()
        {
            ObservableCollection<RegionOption> newRegionOptions = new ObservableCollection<RegionOption>();
            RegionOption selectedRegionOption = null;

            if (ShowRegionSelector && _Chart.Regions != null)
            {
                string lastRegionSelected = Properties.Settings.Default.LastRegionSelected;

                foreach (var kvp in _Chart.Regions)
                {
                    var option = new RegionOption(kvp.Key, kvp.Value);
                    newRegionOptions.Add(option);
                    if (kvp.Key == lastRegionSelected)
                    {
                        selectedRegionOption = option;
                    }
                }
                _Chart.LoadSavedOptions(SelectedRegion?.Key);
            }

            RegionOptions = newRegionOptions;
            SelectedRegion = selectedRegionOption;
        } 

        private void SelectResult(int result)
        {
            var optionDetailModel = _Chart.GetOptionForResult(result);
            var selectedIndex = -1;
            if (optionDetailModel != null)
            {
                foreach (var item in ChartOptions)
                {
                    selectedIndex++;
                    if (item.OptionId == optionDetailModel.OptionId)
                    {
                        item.IsSelected = true;
                        SelectedIndex = selectedIndex;
                        break;
                    }
                }
            }
        }

        private void UpdateChartDetail()
        {
            string description = string.Empty;
            if (_SelectedIndex >= 0 && _SelectedIndex < ChartOptions.Count)
            {
                description = ChartOptions[_SelectedIndex].RawDescription;
            }
            SelectedOptionDescriptionDocument = RichTextStringFormatters.AddLocalLinksToChartKeysDocument(_ChartService, description, ChartHyperLinkCommand);
        }

        private void UpdateNavigationHistory(ChartSelectedEventArgs args)
        {
            if (args.ResetNavigationHistory)
            {
                _NextCharts.Clear();

                if (_Chart != null)
                {
                    _PreviousCharts.Push(_Chart.Key);
                }
            }

            GoToNextCommand.RaiseCanExecuteChanged();
            GoToPreviousCommand.RaiseCanExecuteChanged();
        }
    }

    public class RegionOption
    {
        public string Key { get; private set; }
        public string Description { get; private set; }

        public RegionOption(string key, string description)
        {
            Key = key;
            Description = description;
        }
    }
}
