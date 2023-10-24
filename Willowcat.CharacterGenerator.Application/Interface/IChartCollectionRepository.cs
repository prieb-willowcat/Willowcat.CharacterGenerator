using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Application.Interface
{
    public interface IChartCollectionRepository
    {
        Task<IEnumerable<ChartModel>> BuildChartsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ChartCollectionModel>> BuildCollectionsAsync(CancellationToken cancellationToken = default);
    }
}