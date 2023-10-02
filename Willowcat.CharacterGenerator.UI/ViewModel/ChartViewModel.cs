using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;
using Willowcat.CharacterGenerator.UI.Commands;
using Willowcat.CharacterGenerator.UI.Data;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{

    public class ChartViewModel : ViewModelBase
    {
        private readonly ChartModel _chart = null;
        private readonly ChartService _chartService = null;
        private readonly IEventAggregator _eventAggregator = null;
        private readonly Random _randomizer;

        private bool _IsProcessRunning = false;
        private int _SelectedIndex = -1;
        private int? _Result = null;
        private int? _Modifier = null;
        private FlowDocument _ChartDescriptionDocument = null;
        private FlowDocument _SelectedOptionDescriptionDocument = null;

        public virtual bool CanDynamicallyGenerateOptions => false;

        public ChartModel Chart => _chart;

        public FlowDocument ChartDescriptionDocument
        {
            get => _ChartDescriptionDocument;
            set
            {
                _ChartDescriptionDocument = value;
                OnPropertyChanged();
            }
        }

        public string ChartDice => _chart?.Dice.ToString() ?? string.Empty;

        public ICommand ChartHyperLinkCommand { get; private set; }

        public string ChartLocationString => string.Join(" > ", _chartService.GetChartHierarchyPath(_chart));

        public string ChartName => _chart?.ChartName ?? "Chart";

        public ObservableCollection<ChartOptionDetailModel> ChartOptions { get; private set; } = new ObservableCollection<ChartOptionDetailModel>();

        public AsyncCommand GenerateOptionsCommand { get; private set; }

        public bool IsProcessRunning
        {
            get => _IsProcessRunning;
            set
            {
                _IsProcessRunning = value;
                OnPropertyChanged();
            }
        }

        public string Key => _chart?.Key;

        public int? Modifier
        {
            get => _Modifier;
            set
            {
                _Modifier = value;
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

        public virtual ObservableCollection<RegionOption> RegionOptions { get; set; } = null;

        public ICommand RollCommand { get; private set; }

        public virtual RegionOption SelectedRegion { get; set; } = null;

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

        public virtual bool ShowRegionSelector => false;

        public ICommand UseSelectionCommand { get; private set; }

        public ChartViewModel(ChartModel chart, Random randomizer, ChartService chartService, IEventAggregator eventAggregator)
        {
            _chart = chart;
            _randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            _chartService = chartService ?? throw new ArgumentNullException(nameof(chartService));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            ChartHyperLinkCommand = new DelegateCommand<string>(OnChartHyperLinkExecute);
            GenerateOptionsCommand = new AsyncCommand(OnGenerateOptionsExecute, OnGenerateOptionsCanExecute);
            RollCommand = new DelegateCommand(OnRollExecute);
            UseSelectionCommand = new DelegateCommand(OnUseSelectionExecute);
        }

        protected virtual Task GenerateOptionsAsync(Random randomizer)
        {
            return Task.CompletedTask;
        }

        private void OnChartHyperLinkExecute(string chartKey)
        {
            _eventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(chartKey));
        }

        private bool OnGenerateOptionsCanExecute() => CanDynamicallyGenerateOptions;

        private async Task OnGenerateOptionsExecute()
        {
            if (CanDynamicallyGenerateOptions)
            {
                try
                {
                    IsProcessRunning = true;
                    await GenerateOptionsAsync(_randomizer);
                    OnPropertyChanged(nameof(ChartDice));
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
        }

        private void OnRollExecute()
        {
            if (_chart != null)
            {
                int result = _chart.Dice.Roll(_randomizer, Modifier ?? 0);
                Result = result;
                SelectResult(result);
            }
        }

        private void OnUseSelectionExecute()
        {
            if (SelectedIndex >= 0 && SelectedIndex < ChartOptions.Count)
            {
                Guid selectedId = ChartOptions[SelectedIndex].OptionId;
                var selectedOption = _chart.GetSelectedOption(selectedId);
                _eventAggregator.GetEvent<OptionSelectedEvent>().Publish(new OptionSelectedEventArgs(selectedOption));
            }
        }

        public virtual void Initialize(int? selectedValue) 
        {
            InitializeOptions(selectedValue);
        }

        protected void InitializeOptions(int? selectedValue)
        {
            ChartOptions.Clear();
            if (_chart != null)
            {
                foreach (var option in _chart.Options.OrderBy(x => x.Range.Start))
                {
                    ChartOptions.Add(new ChartOptionDetailModel(_chartService, _eventAggregator, option));
                }
                ChartDescriptionDocument = RichTextStringFormatters.AddLocalLinksToChartKeysDocument(_chartService, _chart.Notes, ChartHyperLinkCommand);
                if (selectedValue.HasValue)
                {
                    SelectResult(selectedValue.Value);
                }
            }
        }

        private void SelectResult(int result)
        {
            var optionDetailModel = _chart.GetOptionForResult(result);
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
            SelectedOptionDescriptionDocument = RichTextStringFormatters.AddLocalLinksToChartKeysDocument(_chartService, description, ChartHyperLinkCommand);
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
