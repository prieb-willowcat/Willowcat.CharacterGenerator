using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core
{
    public interface IAutoGeneratorFactory
    {
        public bool CanAutoGenerate(ChartModel chart);

        public ChartModel GetAutoGeneratingChart(ChartModel chart);
    }
}