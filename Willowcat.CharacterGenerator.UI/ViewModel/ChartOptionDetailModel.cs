using Prism.Events;
using System;
using System.Diagnostics;
using Willowcat.CharacterGenerator.EntityFramework.Repository;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.UI.Data;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    [DebuggerDisplay("{Range} - {RawDescription} (OptionId={OptionId})")]
    public class ChartOptionDetailModel : ViewModelBase
    {
        private readonly ChartService _ChartService = null;
        private readonly OptionModel _OptionModel = null;
        private bool _IsSelected = false;

        public string FormattedDescription { get; set; }
        public GoToChartViewModel GoToChartViewModel { get; set; }
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                OnPropertyChanged();
            }
        }
        public Guid OptionId => _OptionModel.OptionId;
        public string Range => _OptionModel.Range.ToString();
        public string RawDescription => _OptionModel.Description;

        public ChartOptionDetailModel(ChartService chartService, IEventAggregator eventAggregator, OptionModel option)
        {
            _ChartService = chartService;
            _OptionModel = option;
            InitializeGoToChartModel(eventAggregator, option);
            FormattedDescription = RichTextStringFormatters.AddLinks(chartService, RawDescription);
        }

        private void InitializeGoToChartModel(IEventAggregator eventAggregator, OptionModel option)
        {
            ChartModel gotoChart = null;
            if (!string.IsNullOrEmpty(option.GoToChartKey))
            {
                gotoChart = _ChartService.GetChart(option.GoToChartKey);
            }
            GoToChartViewModel = new GoToChartViewModel(eventAggregator, gotoChart);
        }
    }
}
