using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class ChartCollectionViewModel : ViewModelBase
    {
        private readonly IEventAggregator _EventAggregator = null;
        private bool _IsExpanded = false;
        private readonly List<ChartListChartViewModel> _Charts = new List<ChartListChartViewModel>();

        public ObservableCollection<ViewModelBase> FilteredItems { get; private set; } = new ObservableCollection<ViewModelBase>();

        public string CollectionName { get; set; }

        public int FilteredCount => FilteredItems.Sum((x) => {
            if (x is ChartCollectionViewModel collection) return collection.FilteredCount;
            else if (x is ChartListChartViewModel chart) return 1;
            else return 0;
        });

        public bool HasMatchingItems => FilteredCount > 0;

        public bool IsExpanded
        {
            get => _IsExpanded;
            set
            {
                _IsExpanded = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChartCollectionViewModel> SubCollections { get; private set; } = new ObservableCollection<ChartCollectionViewModel>();

        public ChartCollectionViewModel(IEventAggregator eventAggregator, ChartCollectionModel baseModel)
        {
            _EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _EventAggregator.GetEvent<ChartSelectedEvent>().Subscribe(OnChartSelectedEvent);
            _EventAggregator.GetEvent<ApplyChartFilterEvent>().Subscribe(OnApplyChartFilterEvent);

            CollectionName = baseModel.CollectionName;
            foreach (var item in baseModel.Charts.Where(x => x.ParentKey == null))
            {
                var chartViewModel = new ChartListChartViewModel(_EventAggregator, item);
                _Charts.Add(chartViewModel);
            }
            foreach (var item in baseModel.SubCollections)
            {
                SubCollections.Add(new ChartCollectionViewModel(_EventAggregator, item));
            }
            OnApplyChartFilterEvent(null);
        }

        public bool ContainsChart(string chartKey)
        {
            return _Charts.Any(chart => chart.ContainsKey(chartKey)) || SubCollections.Any(collection => collection.ContainsChart(chartKey));
        }

        private void OnApplyChartFilterEvent(ApplyChartFilterEventArgs args)
        {
            FilteredItems.Clear();
            foreach (var collection in SubCollections)
            {
                FilteredItems.Add(collection);
            }
            foreach (var chart in _Charts)
            {
                if (args?.ChartFilterViewModel != null)
                {
                    chart.ApplyFilter(args?.ChartFilterViewModel);
                }
                else
                {
                    chart.MatchesFilter = true;
                }
                if (chart.MatchesFilter)
                {
                    FilteredItems.Add(chart);
                }
            }
            OnPropertyChanged(nameof(FilteredCount));
            OnPropertyChanged(nameof(HasMatchingItems));
        }

        private void OnChartSelectedEvent(ChartSelectedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.ChartKey) && ContainsChart(args.ChartKey))
            {
                IsExpanded = true;
            }
        }
    }
}
