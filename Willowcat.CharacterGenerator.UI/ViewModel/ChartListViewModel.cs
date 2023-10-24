using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.EntityFramework.Repository;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class ChartListViewModel : ViewModelBase
    {
        private readonly ChartService _BusinessObject = null;
        private readonly IEventAggregator _EventAggregator = null;

        public ObservableCollection<ChartCollectionViewModel> ChartCollections { get; private set; } = new ObservableCollection<ChartCollectionViewModel>();

        public ChartFilterViewModel ChartFilterViewModel { get; private set; } 

        public ChartListViewModel(TagService tagService, ChartService chartService, IEventAggregator eventAggregator)
        {
            _BusinessObject = chartService ?? throw new ArgumentNullException(nameof(chartService));
            _EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            ChartFilterViewModel = new ChartFilterViewModel(tagService, eventAggregator);
        }

        public async Task LoadChartsAsync()
        {
            try
            {
                //Cursor = Cursors.WaitCursor;
                _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(string.Empty));

                await ChartFilterViewModel.LoadTagsAsync();

                ChartCollections.Clear();
                var chartCollections = await _BusinessObject.LoadChartCollectionsAsync();
                foreach (var collection in chartCollections)
                {
                    if (!collection.HideFromMainScreen)
                    {
                        var chartViewModel = new ChartCollectionViewModel(_EventAggregator, collection);
                        ChartCollections.Add(chartViewModel);
                    }
                }
            }
            finally
            {
                //Cursor = Cursors.Default;
            }
        }
    }
}
