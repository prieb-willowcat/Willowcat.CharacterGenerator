using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;
using Willowcat.Common.Utilities;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class ChartListChartViewModel : ViewModelBase
    {
        private readonly ChartModel _Chart = null;
        private readonly IEventAggregator _EventAggregator = null;

        private bool _IsExtended = false;
        private bool _IsSelected = false;
        private bool _MatchesFilter = true;

        public string ChartName => _Chart.ChartName;

        public string ChartKey => _Chart.Key;

        public bool IsExtended
        {
            get => _IsExtended;
            set
            {
                _IsExtended = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                FireSelectChart(value);
            }
        }

        public bool MatchesFilter
        {
            get => _MatchesFilter;
            set
            {
                _MatchesFilter = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChartListChartViewModel> SubCharts { get; private set; } = new ObservableCollection<ChartListChartViewModel>();

        public ChartListChartViewModel(IEventAggregator eventAggregator, ChartModel chartModel)
        {
            _EventAggregator = eventAggregator;
            _Chart = chartModel;
            _EventAggregator.GetEvent<ChartSelectedEvent>().Subscribe(OnChartSelectedEvent);
            foreach (var item in chartModel.SubCharts)
            {
                var chartViewModel = new ChartListChartViewModel(_EventAggregator, item);
                SubCharts.Add(chartViewModel);
            }
        }

        public void ApplyFilter(ChartFilterViewModel filterViewModel)
        {
            if (filterViewModel.SelectedTag == null || filterViewModel.SelectedTag.TagId < 0)
            {
                MatchesFilter = true;
            }
            else
            {
                MatchesFilter = _Chart.Tags.Any(tag => tag.TagId == filterViewModel.SelectedTag.TagId);
                foreach (var chart in SubCharts)
                {
                    chart.MatchesFilter = true;
                }
            }
        }

        public bool ContainsKey(string chartKey)
        {
            return !string.IsNullOrEmpty(chartKey) && 
                ChartKey.EqualsIgnoreCase(chartKey) && 
                SubCharts.Any(chart => chart.ContainsKey(chartKey));
        }

        private void FireSelectChart(bool value)
        {
            if (_IsSelected != value)
            {
                _IsSelected = value;
                OnPropertyChanged();
                if (_IsSelected)
                {
                    _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(ChartKey));
                }
            }
        }

        private void OnChartSelectedEvent(ChartSelectedEventArgs args)
        {
            if (ContainsKey(args.ChartKey))
            {
                IsExtended = true;
                _IsSelected = ChartKey.EqualsIgnoreCase(args.ChartKey);
            }
            else
            {
                _IsSelected = false;
            }
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}
