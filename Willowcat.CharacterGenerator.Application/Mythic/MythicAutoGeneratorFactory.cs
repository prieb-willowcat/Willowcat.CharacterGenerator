using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Application.Mythic
{
    public class MythicAutoGeneratorFactory : IAutoGeneratorFactory, IChartCollectionRepository
    {
        private readonly List<MythicFateChart> _fateCharts = new List<MythicFateChart>();

        public bool CanAutoGenerate(ChartModel chart) => chart.Key.StartsWith("MythicFate", StringComparison.InvariantCulture);

        public ChartModel GetAutoGeneratingChart(ChartModel chart)
        {
            if (CanAutoGenerate(chart))
            {
                var charts = GetMythicFateCharts();
                var matching = charts.FirstOrDefault(x => x.Key == chart.Key);
                if (matching != null)
                {
                    chart = matching;
                }
            }
            return chart;
        }

        public IEnumerable<ChartModel> GetMythicFateCharts()
        {
            if (!_fateCharts.Any())
            {
                for (int chaosRank = 1; chaosRank <= 9; chaosRank++)
                {
                    var rankChart = new MythicFateChart(chaosRank);
                    _fateCharts.Add(rankChart);
                    foreach (var odds in Enum.GetValues(typeof(MythicFateOdds)))
                    {
                        var oddsChart = new MythicFateChart(chaosRank, (MythicFateOdds)odds);
                        rankChart.SubCharts.Add(oddsChart);
                        _fateCharts.Add(oddsChart);
                    }
                }
            }
            return _fateCharts;
        }

        public Task<IEnumerable<ChartModel>> BuildChartsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetMythicFateCharts());
        }

        public Task<IEnumerable<ChartCollectionModel>> BuildCollectionsAsync(CancellationToken cancellationToken = default)
        {
            // TODO: don't create this in two places
            IEnumerable<ChartCollectionModel> collections = new ChartCollectionModel[]
            {
                new ChartCollectionModel()
                {
                    CollectionName = "Mythic",
                    CollectionId = "MythicRandomEventCharts",
                    CollectionTag = "Event",
                    FileName = "MythicRandomEventCharts",
                    Sequence = 10
                }
            };
            return Task.FromResult(collections);
        }
    }
}
