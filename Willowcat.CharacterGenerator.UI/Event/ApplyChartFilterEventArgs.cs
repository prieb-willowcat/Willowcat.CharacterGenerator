using Willowcat.CharacterGenerator.UI.ViewModel;

namespace Willowcat.CharacterGenerator.UI.Event
{
    public class ApplyChartFilterEventArgs
    {
        public ChartFilterViewModel ChartFilterViewModel { get; private set; }

        public ApplyChartFilterEventArgs(ChartFilterViewModel viewModel)
        {
            ChartFilterViewModel = viewModel;
        }
    }
}
