using Prism.Events;
using System;
using Willowcat.CharacterGenerator.EntityFramework.Repository;
using Willowcat.CharacterGenerator.OnlineGenerators;

namespace Willowcat.CharacterGenerator.UI.ViewModel.Factory
{
    public class ChartViewModelFactory
    {
        private readonly ChartService _chartService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Random _randomizer;

        public ChartViewModelFactory(Random randomizer, ChartService chartService, IEventAggregator eventAggregator)
        {
            _randomizer = randomizer;
            _chartService = chartService;
            _eventAggregator = eventAggregator;
        }

        public ChartViewModel CreateViewModelFromKey(string key)
        {
            ChartViewModel viewModel = null;
            if (!string.IsNullOrEmpty(key))
            {
                var chart = _chartService.GetChart(key);
                if (chart is RandomNameChart randomNameChart)
                {
                    viewModel = new RandomNameChartViewModel(randomNameChart, _randomizer, _chartService, _eventAggregator);
                }
                else
                {
                    viewModel = new ChartViewModel(chart, _randomizer, _chartService, _eventAggregator);
                }
            }
            return viewModel;
        }
    }
}
