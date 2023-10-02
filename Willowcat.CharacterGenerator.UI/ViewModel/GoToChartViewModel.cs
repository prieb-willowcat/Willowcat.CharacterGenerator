using Prism.Commands;
using Prism.Events;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class GoToChartViewModel : ViewModelBase
    {
        private readonly IEventAggregator _EventAggregator = null;
        private readonly ChartModel _Chart;

        public string GoToChartKey => _Chart?.Key ?? string.Empty;
        public string GoToChartName => _Chart?.ChartName ?? string.Empty;
        public bool HasGoToChartOption => _Chart != null;

        public ICommand NavigateToChartCommand { get; private set; }

        public GoToChartViewModel(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
        }

        public GoToChartViewModel(IEventAggregator eventAggregator, ChartModel chart = null)
        {
            _EventAggregator = eventAggregator;
            _Chart = chart;

            if (_Chart != null)
            {
                NavigateToChartCommand = new DelegateCommand(OnNavigateToChartExecute);
            }            
        }

        private void OnNavigateToChartExecute()
        {
            if (!string.IsNullOrEmpty(GoToChartKey))
            {
                _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(GoToChartKey));
            }
        }
    }
}
