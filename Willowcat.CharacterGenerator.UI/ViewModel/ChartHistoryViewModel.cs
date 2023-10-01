using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class ChartHistoryViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator = null;

        private readonly Stack<string> _nextCharts = new();
        private readonly Stack<string> _previousCharts = new();

        private string _currentChartKey = string.Empty;

        public ChartHistoryViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            GoToNextCommand = new DelegateCommand(OnGoToNextExecute, OnGoToNextCanExecute);
            GoToPreviousCommand = new DelegateCommand(OnGoToPreviousExecute, OnGoToPreviousCanExecute);

            _eventAggregator.GetEvent<ChartSelectedEvent>().Subscribe(OnChartSelectedEvent);
        }

        public DelegateCommand GoToNextCommand { get; private set; }

        public DelegateCommand GoToPreviousCommand { get; private set; }

        private void OnChartSelectedEvent(ChartSelectedEventArgs args)
        {
            if (args.ResetNavigationHistory)
            {
                _nextCharts.Clear();

                if (!string.IsNullOrEmpty(_currentChartKey))
                {
                    _previousCharts.Push(_currentChartKey);
                }
            }
            _currentChartKey = args.ChartKey;

            GoToNextCommand.RaiseCanExecuteChanged();
            GoToPreviousCommand.RaiseCanExecuteChanged();
        }

        private bool OnGoToNextCanExecute() => _nextCharts.Any();

        private void OnGoToNextExecute()
        {
            if (_nextCharts.Any())
            {
                if (!string.IsNullOrEmpty(_currentChartKey))
                {
                    _previousCharts.Push(_currentChartKey);
                }
                string nextChartKey = _nextCharts.Pop();
                _eventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(nextChartKey, false));
            }
        }

        private bool OnGoToPreviousCanExecute() => _previousCharts.Any();

        private void OnGoToPreviousExecute()
        {
            if (_previousCharts.Any())
            {
                if (!string.IsNullOrEmpty(_currentChartKey))
                {
                    _nextCharts.Push(_currentChartKey);
                }
                string previousChartKey = _previousCharts.Pop();
                _eventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(previousChartKey, false));
            }
        }
    }
}
