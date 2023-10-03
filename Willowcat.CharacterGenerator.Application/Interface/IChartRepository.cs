using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Application.Interface
{
    public interface IChartRepository
    {
        ChartModel? GetChart(string chartKey);

        ChartCollectionModel? GetCollection(string source);

        List<string> GetChartHierarchyPath(ChartModel chart);

        Task<List<ChartCollectionModel>> LoadChartCollectionsAsync();
    }
}