using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.OnlineGenerators.Generator;

namespace Willowcat.CharacterGenerator.OnlineGenerators
{
    public class RandomNameChartFactory : IChartCollectionRepository, IAutoGeneratorFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<RandomNameChartFactory> _logger;
        private readonly Func<string> _getBehindTheNameApiKey;
        private Dictionary<string, RandomNameChart>? _nameCharts = null;
        private static readonly object _lock = new object(); 

        public RandomNameChartFactory(IServiceProvider provider, ILogger<RandomNameChartFactory> logger, Func<string> getBehindTheNameApiKey)
        {
            _provider = provider;
            _logger = logger;
            _getBehindTheNameApiKey = getBehindTheNameApiKey;
        }

        public Task<IEnumerable<ChartModel>> BuildChartsAsync(CancellationToken cancellationToken = default)
        {
            InitializeCharts();
            IEnumerable<ChartModel> charts = Array.Empty<ChartModel>();
            if (_nameCharts != null)
            {
                charts = _nameCharts.Values;
            }
            return Task.FromResult(charts);
        }

        public Task<IEnumerable<ChartCollectionModel>> BuildCollectionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IEnumerable<ChartCollectionModel>)Array.Empty<ChartCollectionModel>());
        }

        public bool CanAutoGenerate(ChartModel chart) => chart.Source == "Names";

        public ChartModel GetAutoGeneratingChart(ChartModel chart)
        {
            InitializeCharts();
            if (_nameCharts != null && _nameCharts.TryGetValue(chart.Key, out RandomNameChart? randomNameChart))
            {
                if (randomNameChart != null)
                {
                    chart = randomNameChart;
                }
            }

            return chart;
        }

        private RandomNameChart? CreateRandomNameChart(NameCategory nameCategory)
        {
            string? key = _getBehindTheNameApiKey != null ? _getBehindTheNameApiKey() : null;
            var httpClient = _provider.GetRequiredService<IHttpJsonClient>();
            RandomNameChart? chart = null;
            switch (nameCategory)
            {
                case NameCategory.Elvish:
                    chart = new RandomNameChart(new RandomElvenNames(httpClient), "names-elven", "Elf Names");
                    break;

                case NameCategory.Human_Female:
                    if (!string.IsNullOrEmpty(key)) 
                    {
                        var generator = new RandomBehindTheName(httpClient, key, Gender.Female, 18);
                        chart = new RandomNameChart(generator, "names-female", "Female Names");
                    }
                    break;

                case NameCategory.Human_Male:
                    if (!string.IsNullOrEmpty(key))
                    {
                        var generator = new RandomBehindTheName(httpClient, key, Gender.Male, 18);
                        chart = new RandomNameChart(generator, "names-male", "Male Names");
                    }
                    break;
            }
            return chart;
        }

        private void InitializeCharts()
        {
            lock (_lock)
            {
                if (_nameCharts == null)
                {
                    _nameCharts = new Dictionary<string, RandomNameChart>();

                    try
                    {
                        var categories = Enum.GetValues<NameCategory>();
                        foreach (var category in categories)
                        {
                            var chart = CreateRandomNameChart(category);
                            if (chart != null)
                            {
                                _nameCharts[chart.Key] = chart;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unable to initialize charts.");
                    }
                }
            }
        }
    }
}
