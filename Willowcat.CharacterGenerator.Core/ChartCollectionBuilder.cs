using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Core.Randomizer;
using Willowcat.CharacterGenerator.Core.TextRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Willowcat.CharacterGenerator.Core.Data;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Willowcat.CharacterGenerator.Core
{
    public class ChartCollectionBuilder
    {
        private readonly ChartFlatFileSerializer _ChartSerializer;
        private readonly Dictionary<string, ChartCollectionModel> _ChartCollectionsByFileName = new Dictionary<string, ChartCollectionModel>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FlatFileChartModel> _Charts = new Dictionary<string, FlatFileChartModel>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _InvalidChartKeys = new HashSet<string>();

        public ChartCollectionBuilder(ChartFlatFileSerializer chartSerializer = null)
        {
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
        }

        private void AddChartsFromFile(string directory, string fileName)
        {
            var path = Path.Combine(directory, fileName + ".txt");
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                var charts = _ChartSerializer.Deserialize(fileName, lines);
                AddCharts(charts, fileName);
            }
        }

        public async Task<ChartCollectionBuilder> AddChartsFromResourcesAsync(DatabaseConfiguration options, IProgress<ChartSetupMessage> progressReporter)
        {
            await Task.Run(() => InitializeFileChartCollections(options));
            progressReporter?.Report(new ChartSetupMessage(this, "Initialized collections", 0, _ChartCollectionsByFileName.Count));

            var resourceFileDirectory = options.GetResourceDirectory();
            var itemsProcessed = 0;
            foreach (var kvp in _ChartCollectionsByFileName)
            {
                itemsProcessed++;
                await Task.Run(() => AddChartsFromFile(resourceFileDirectory, kvp.Key));
                progressReporter?.Report(new ChartSetupMessage(this, $"Adding collection {kvp.Key}", itemsProcessed));
            }
            return this;
        }

        public ChartCollectionBuilder AddChartsFromResources(DatabaseConfiguration options)
        {
            InitializeFileChartCollections(options);

            var resourceFileDirectory = options.GetResourceDirectory();
            foreach (var kvp in _ChartCollectionsByFileName)
            {
                AddChartsFromFile(resourceFileDirectory, kvp.Key);
            }
            return this;
        }

        private void AddCollectionsToDictionary(IEnumerable<ChartCollectionModel> chartCollections, ChartCollectionModel parent = null)
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

        //public ChartCollectionBuilder AddNameCharts(DatabaseConfiguration options)
        //{

        //    AddCharts(new[] {
        //        RandomNameChart.GetFemaleNameChart(options.BehindTheNameApiKey),
        //        RandomNameChart.GetMaleNameChart(options.BehindTheNameApiKey),
        //        RandomNameChart.GetElvenNameChart()
        //    }, "Names");
        //    return this;
        //}

        private void InitializeFileChartCollections(DatabaseConfiguration options)
        {
            _ChartCollectionsByFileName.Clear();
            string resourceFileDirectory = options.GetResourceDirectory();
            string jsonFilePath = Path.Combine(resourceFileDirectory, "ChartCollections.json");
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

        public Dictionary<string, FlatFileChartModel> BuildCharts() => _Charts;

        public List<ChartCollectionModel> BuildCollections()
        {
            return new List<ChartCollectionModel>(_ChartCollectionsByFileName.Values);
        }
    }
}
