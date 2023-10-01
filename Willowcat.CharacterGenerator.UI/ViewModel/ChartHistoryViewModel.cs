using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Core.Randomizer;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class ChartHistoryViewModel : ViewModelBase
    {
        private readonly Random _randomizer;
        private readonly ChartService SelectedChartService = null;
        private readonly IEventAggregator _EventAggregator = null;

        private readonly Stack<string> _NextCharts = new Stack<string>();
        private readonly Stack<string> _PreviousCharts = new Stack<string>();
        private ChartViewModel _selectedChart;

        public ChartHistoryViewModel(Random randomizer, ChartService chartService, IEventAggregator eventAggregator)
        {
            _randomizer = randomizer;
            SelectedChartService = chartService ?? throw new ArgumentNullException(nameof(chartService));
            _EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            GoToNextCommand = new DelegateCommand(OnGoToNextExecute, OnGoToNextCanExecute);
            GoToPreviousCommand = new DelegateCommand(OnGoToPreviousExecute, OnGoToPreviousCanExecute);

            _EventAggregator.GetEvent<ChartSelectedEvent>().Subscribe(OnChartSelectedEvent);
        }

        public DelegateCommand GoToNextCommand { get; private set; }

        public DelegateCommand GoToPreviousCommand { get; private set; }

        public ChartViewModel SelectedChart
        {
            get => _selectedChart; 
            set
            {
                _selectedChart = value;
                OnPropertyChanged();
            }
        }

        private void OnChartSelectedEvent(ChartSelectedEventArgs args)
        {
            UpdateNavigationHistory(args);

            var chart = SelectedChartService.GetChart(args.ChartKey);
            if (chart is RandomNameChart randomNameChart)
            {
                SelectedChart = new RandomNameChartViewModel(randomNameChart, SelectedChartService, _EventAggregator);
            }
            else
            {
                SelectedChart = new ChartViewModel(chart, SelectedChartService, _EventAggregator);
            }

            if (SelectedChart != null)
            {
                SelectedChart.Randomizer = _randomizer;
                SelectedChart.Initialize(args.Range);
            }
        }

        private bool OnGoToNextCanExecute() => _NextCharts.Any();

        private void OnGoToNextExecute()
        {
            if (_NextCharts.Any())
            {
                if (SelectedChart != null)
                {
                    _PreviousCharts.Push(SelectedChart.Key);
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
                if (SelectedChart != null)
                {
                    _NextCharts.Push(SelectedChart.Key);
                }
                string previousChartKey = _PreviousCharts.Pop();
                _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(previousChartKey, false));
            }
        }

        private void UpdateNavigationHistory(ChartSelectedEventArgs args)
        {
            if (args.ResetNavigationHistory)
            {
                _NextCharts.Clear();

                if (!string.IsNullOrEmpty(SelectedChart?.Key))
                {
                    _PreviousCharts.Push(SelectedChart.Key);
                }
            }

            GoToNextCommand.RaiseCanExecuteChanged();
            GoToPreviousCommand.RaiseCanExecuteChanged();
        }

    }
}
