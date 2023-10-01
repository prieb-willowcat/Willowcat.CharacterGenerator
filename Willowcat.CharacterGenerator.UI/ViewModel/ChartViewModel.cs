using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Models;
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
        private readonly ChartModel _Chart = null;
        private readonly ChartService _ChartService = null;
        private readonly IEventAggregator _EventAggregator = null;

        private bool _IsProcessRunning = false;
        private int _SelectedIndex = -1;
        private int? _Result = null;
        private int? _Modifier = null;
        private FlowDocument _ChartDescriptionDocument = null;
        private FlowDocument _SelectedOptionDescriptionDocument = null;

        public virtual bool CanDynamicallyGenerateOptions => false;

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

        public string ChartDice => _Chart?.Dice.ToString() ?? string.Empty;

        public ICommand ChartHyperLinkCommand { get; private set; }

        public string ChartLocationString => string.Join(" > ", _ChartService.GetChartHierarchyPath(_Chart));

        public string ChartName => _Chart?.ChartName ?? "Chart";

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

        public string Key => _Chart?.Key;

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

        public ChartViewModel(ChartModel chart, ChartService chartService, IEventAggregator eventAggregator)
        {
            _Chart = chart;
            _ChartService = chartService ?? throw new ArgumentNullException(nameof(chartService));
            _EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

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
            _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(chartKey));
        }

        private bool OnGenerateOptionsCanExecute() => CanDynamicallyGenerateOptions;

        private async Task OnGenerateOptionsExecute()
        {
            if (CanDynamicallyGenerateOptions)
            {
                try
                {
                    IsProcessRunning = true;
                    await GenerateOptionsAsync(Randomizer);
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
            if (_Chart != null)
            {
                int result = _Chart.Dice.Roll(Randomizer, Modifier ?? 0);
                Result = result;
                SelectResult(result);
            }
        }

        private void OnUseSelectionExecute()
        {
            if (SelectedIndex >= 0 && SelectedIndex < ChartOptions.Count)
            {
                Guid selectedId = ChartOptions[SelectedIndex].OptionId;
                var selectedOption = _Chart.GetSelectedOption(selectedId);
                _EventAggregator.GetEvent<OptionSelectedEvent>().Publish(new OptionSelectedEventArgs(selectedOption));
            }
        }

        public virtual void Initialize(int? selectedValue) 
        {
            InitializeOptions(selectedValue);
        }

        protected void InitializeOptions(int? selectedValue)
        {
            ChartOptions.Clear();
            if (_Chart != null)
            {
                foreach (var option in _Chart.Options.OrderBy(x => x.Range.Start))
                {
                    ChartOptions.Add(new ChartOptionDetailModel(_ChartService, _EventAggregator, option));
                }
                ChartDescriptionDocument = RichTextStringFormatters.AddLocalLinksToChartKeysDocument(_ChartService, _Chart.Notes, ChartHyperLinkCommand);
                if (selectedValue.HasValue)
                {
                    SelectResult(selectedValue.Value);
                }
            }
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
