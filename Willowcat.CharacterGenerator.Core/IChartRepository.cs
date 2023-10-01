using System.Collections.Generic;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core
{
    public interface IChartRepository
    {
        ChartModel GetChart(string chartKey);
        ChartCollectionModel GetCollection(string source);
        List<string> GetChartHierarchyPath(ChartModel chart);
        Task<List<ChartCollectionModel>> LoadChartCollectionsAsync();
    }
}