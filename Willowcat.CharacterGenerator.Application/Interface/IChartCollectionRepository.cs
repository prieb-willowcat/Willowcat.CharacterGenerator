using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core
{
    public interface IChartCollectionRepository
    {
        Task<IEnumerable<ChartModel>> BuildChartsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ChartCollectionModel>> BuildCollectionsAsync(CancellationToken cancellationToken = default);
    }
}