using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.EntityFramework.Repository;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class ChartFilterViewModel : ViewModelBase
    {
        private readonly TagService _TagService;
        private readonly IEventAggregator _EventAggregator = null;
        private TagModel _SelectedTag = null;
        private ObservableCollection<TagModel> _Tags = new ObservableCollection<TagModel>();

        public ObservableCollection<TagModel> Tags
        {
            get => _Tags; 
            private set
            {
                _Tags = value;
                OnPropertyChanged();
            }
        }

        public TagModel SelectedTag
        {
            get => _SelectedTag;
            set
            {
                _SelectedTag = value;
                OnPropertyChanged();
                _EventAggregator.GetEvent<ApplyChartFilterEvent>().Publish(new ApplyChartFilterEventArgs(this));
            }
        }

        public ChartFilterViewModel(TagService tagService, IEventAggregator eventAggregator)
        {
            _TagService = tagService;
            _EventAggregator = eventAggregator;
        }

        public async Task LoadTagsAsync()
        {
            var tags = await _TagService.GetTagsAsync();
            Tags = new ObservableCollection<TagModel>(tags);
            Tags.Insert(0, new TagModel()
            {
                Name = "All",
                TagId = -1
            });
            SelectedTag = null;
        }
    }
}
