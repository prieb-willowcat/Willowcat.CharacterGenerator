using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Application.Interface
{
    public interface IAutoGeneratorFactory
    {
        public bool CanAutoGenerate(ChartModel chart);

        public ChartModel GetAutoGeneratingChart(ChartModel chart);
    }
}