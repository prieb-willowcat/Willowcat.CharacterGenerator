using System.Text.Json;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Progress;

namespace Willowcat.CharacterGenerator.FlatFile.Repository
{
    public class ChartCollectionBuilder : IChartCollectionRepository
    {
        private readonly IProgress<ChartSetupMessage>? _progressReporter;
        private readonly string _resourceDirectory;
        private readonly ChartFlatFileSerializer _ChartSerializer;
        private readonly Dictionary<string, ChartCollectionModel> _ChartCollectionsByFileName = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FlatFileChartModel> _Charts = new(StringComparer.OrdinalIgnoreCase);

        public ChartCollectionBuilder(
            string resourceDirectory,
            ChartFlatFileSerializer? chartSerializer = null,
            IProgress<ChartSetupMessage>? progressReporter = null)
        {
            _progressReporter = progressReporter;
            _resourceDirectory = resourceDirectory;
            _ChartSerializer = chartSerializer ?? new ChartFlatFileSerializer();
        }

        private void AddCharts(IEnumerable<FlatFileChartModel> charts, string fileName)
        {
            var chartCollection = _ChartCollectionsByFileName[fileName];

            foreach (var chart in charts)
            {
                if (string.IsNullOrEmpty(chart.Key))
                {
                    throw new NullReferenceException($"chart.Key cannot be null: ChartName='{chart.ChartName}' in file {fileName}");
                }
                _Charts[chart.Key] = chart;
                chart.Source = fileName;
            }

            foreach (var chart in charts)
            {
                if (!string.IsNullOrEmpty(chart.ParentKey) && _Charts.ContainsKey(chart.ParentKey))
                {
                    var parentChart = _Charts[chart.ParentKey];
                    parentChart.SubCharts.Add(chart);
                    if (parentChart is FlatFileChartModel parentModel && chart is FlatFileChartModel childModel)
                    {
                        foreach (var tag in parentChart.ParsedTags)
                        {
                            childModel.ParsedTags.Add(tag);
                        }
                    }
                }
                else
                {
                    chartCollection.Charts.Add(chart);
                }

                if (!string.IsNullOrEmpty(chartCollection.CollectionTag) && !chart.ParsedTags.Any())
                {
                    chart.ParsedTags.Add(chartCollection.CollectionTag);
                }
            }
        }

        private void AddChartsFromFile(string fileName)
        {
            var path = Path.Combine(_resourceDirectory, fileName + ".txt");
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                var charts = _ChartSerializer.Deserialize(fileName, lines);
                AddCharts(charts, fileName);
            }
        }

        public async Task<ChartCollectionBuilder> AddChartsFromResourcesAsync()
        {
            await Task.Run(() => InitializeFileChartCollections());
            _progressReporter?.Report(new ChartSetupMessage(this, "Initialized collections", 0, _ChartCollectionsByFileName.Count));

            var itemsProcessed = 0;
            foreach (var kvp in _ChartCollectionsByFileName)
            {
                itemsProcessed++;
                await Task.Run(() => AddChartsFromFile(kvp.Key));
                _progressReporter?.Report(new ChartSetupMessage(this, $"Adding collection {kvp.Key}", itemsProcessed));
            }
            return this;
        }

        private void AddCollectionsToDictionary(IEnumerable<ChartCollectionModel> chartCollections, ChartCollectionModel? parent = null)
        {
            int index = 0;
            foreach (var collection in chartCollections)
            {
                collection.ParentCollection = parent;
                collection.Sequence = index;
                index++;

                if (!string.IsNullOrEmpty(collection.FileName))
                {
                    _ChartCollectionsByFileName[collection.FileName] = collection;
                }

                if (collection.SubCollections.Any())
                {
                    AddCollectionsToDictionary(collection.SubCollections, collection);
                }
            }
        }

        //public ChartCollectionBuilder AddMythicCharts(DatabaseConfiguration _)
        //{
        //    AddCharts(MythicFateChart.GetMythicFateCharts(), "MythicRandomEventCharts");
        //    return this;
        //}

        public Task<IEnumerable<ChartModel>> BuildChartsAsync(CancellationToken cancellationToken = default)
        {
            SetUpTags(_Charts.Values);
            IEnumerable<ChartModel> charts = new List<ChartModel>(_Charts.Values);
            return Task.FromResult(charts);
        }

        public async Task<IEnumerable<ChartCollectionModel>> BuildCollectionsAsync(CancellationToken cancellationToken = default)
        {
            await AddChartsFromResourcesAsync();
            cancellationToken.ThrowIfCancellationRequested();
            return new List<ChartCollectionModel>(_ChartCollectionsByFileName.Values);
        }

        private void InitializeFileChartCollections()
        {
            _ChartCollectionsByFileName.Clear();
            string jsonFilePath = Path.Combine(_resourceDirectory, "ChartCollections.json");
            if (File.Exists(jsonFilePath))
            {
                var chartCollections = JsonSerializer.Deserialize<List<ChartCollectionModel>>(File.ReadAllText(jsonFilePath));
                if (chartCollections != null)
                {
                    AddCollectionsToDictionary(chartCollections);
                }
            }
            else
            {
                throw new FileNotFoundException(jsonFilePath);
            }
        }

        private Dictionary<string, TagModel> SetUpTags(IEnumerable<ChartModel> charts, Dictionary<string, TagModel>? tags = null)
        {
            if (tags == null)
            {
                tags = new Dictionary<string, TagModel>(StringComparer.CurrentCultureIgnoreCase);
            }

            foreach (var chart in charts)
            {
                if (chart is FlatFileChartModel flatFileChart)
                {
                    foreach (var tagName in flatFileChart.ParsedTags)
                    {
                        TagModel? tag = null;
                        if (!tags.TryGetValue(tagName, out tag))
                        {
                            tag = new TagModel()
                            {
                                //TagId = tags.Count,
                                Name = tagName,
                            };
                            tags.Add(tagName, tag);
                        }
                        chart.Tags.Add(tag);
                    }
                }
                tags = SetUpTags(chart.SubCharts, tags);
            }

            return tags;
        }
    }
}
