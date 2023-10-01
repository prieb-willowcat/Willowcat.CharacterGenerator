using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Core.Data;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Core.TextRepository;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;

namespace Willowcat.CharacterGenerator.Core
{
    public class ChartService : IChartRepository
    {
        private readonly DatabaseConfiguration _Options;

        public ChartService(DatabaseConfiguration options)
        {
            _Options = options;
        }

        public ChartModel GetChart(string chartKey)
        {
            ChartModel model = null;
            if (!string.IsNullOrEmpty(chartKey))
            {
                using (ChartContext context = new ChartContext(_Options))
                {
                    model = context.Charts.Find(chartKey);
                    if (model != null)
                    {
                        context.Entry(model)
                            .Collection(x => x.Options)
                            .Load();
                    }
                }
            }
            return model;
        }

        public ChartCollectionModel GetCollection(string source)
        {
            ChartCollectionModel model = null;
            if (!string.IsNullOrEmpty(source))
            {
                using (ChartContext context = new ChartContext(_Options))
                {
                    model = context.ChartCollections.Find(source);
                }
            }
            return model;
        }

        public List<string> GetChartHierarchyPath(ChartModel chart)
        {
            List<string> path = new List<string>();
            if (chart != null)
            {
                ChartModel currentChart = chart;
                while (!string.IsNullOrEmpty(currentChart.ParentKey))
                {
                    var parentChart = GetChart(currentChart.ParentKey);
                    if (parentChart == null) break;

                    currentChart = parentChart;
                    path.Insert(0, currentChart.ChartName);
                }

                var collection = GetCollection(currentChart.Source);
                while (collection != null)
                {
                    path.Insert(0, collection.CollectionName);

                    collection = collection.ParentCollectionId != null ? GetCollection(collection.ParentCollectionId) : null;
                }
            }
            return path;
        }

        public async Task<List<ChartCollectionModel>> LoadChartCollectionsAsync()
        {
            var collections = new List<ChartCollectionModel>();

            using (ChartContext context = new ChartContext(_Options))
            {
                collections = await LoadChartCollections(context);
            }

            return collections;
        }

        private async Task<List<ChartCollectionModel>> LoadChartCollections(ChartContext context, ChartCollectionModel parent = null)
        {
            var collections = new List<ChartCollectionModel>();

            IQueryable<ChartCollectionModel> filtered = null;
            if (parent == null) 
            {
                filtered = context.ChartCollections.Where(x => x.ParentCollection == null);
            }
            else
            {
                filtered = context.ChartCollections.Where(x => x.ParentCollection.CollectionId == parent.CollectionId);
            }

            collections = await filtered
                .Include(x => x.Charts.Where(ch => ch.ParentKey == null))
                .ThenInclude(x => x.Tags)
                .ToListAsync();

            foreach (var sub in collections)
            {
                sub.SubCollections = await LoadChartCollections(context, sub);
                await LoadSubCharts(context, sub.Charts.ToArray());
            }

            return collections;
        }

        private async Task LoadSubCharts(ChartContext context, IEnumerable<ChartModel> charts)
        {
            foreach (var chart in charts)
            {
                chart.SubCharts = await context.Charts
                    .Where(x => x.ParentKey == chart.Key)
                    .Include(x => x.Tags)
                    .ToListAsync();
            }
        }
    }


    [Obsolete]
    public class ChartServiceOld 
    {
        private readonly ChartFlatFileSerializer _ChartSerializer = new ChartFlatFileSerializer();
        private readonly HashSet<string> _InvalidChartKeys = new HashSet<string>();
        
        public Dictionary<string, ChartModel> Charts { get; private set; } = new Dictionary<string, ChartModel>(StringComparer.OrdinalIgnoreCase);
        public List<ChartCollectionModel> ParentChartCollections { get; private set; } = new List<ChartCollectionModel>();
        
        private string ExtractTag(string[] tokens)
        {
            string result = null;
            //Tags#Personality
            foreach (var token in tokens.Skip(1))
            {
                if (token.StartsWith("Tags#"))
                {
                    result = token.Substring("Tags#".Length);
                    break;
                }
            }
            return result;
        }

        public ChartModel GetChart(string chartKey)
        {
            ChartModel model = null;
            if (Charts != null && !string.IsNullOrEmpty(chartKey) && Charts.ContainsKey(chartKey))
            {
                model = Charts[chartKey];
            }
            if (model == null)
            {
                _InvalidChartKeys.Add(chartKey);
            }
            return model;
        }

        public List<string> GetChartHierarchyPath(ChartModel chart)
        {
            List<string> path = new List<string>();
            if (chart != null)
            {
                ChartModel currentChart = chart;
                while (!string.IsNullOrEmpty(currentChart.ParentKey) && Charts.ContainsKey(currentChart.ParentKey))
                {
                    currentChart = Charts[currentChart.ParentKey];
                    path.Insert(0, currentChart.ChartName);
                }

                string source = currentChart.Source;
                var parentCollection = ParentChartCollections.FirstOrDefault(x => x.FileName == source);
                if (parentCollection != null)
                {
                    path.Insert(0, parentCollection.CollectionName);

                    foreach (var parent in ParentChartCollections)
                    {
                        if (parent.SubCollections.Any(x => x.FileName == source))
                        {
                            path.Insert(0, parent.CollectionName);
                        }
                    }
                }
            }
            return path;
        }

        public List<SelectedOption> GetRandomResult(Random randomizer, ChartModel chart)
        {
            List<SelectedOption> Result = new List<SelectedOption>();
            ChartModel NextChart = chart;
            while (NextChart != null)
            {
                int RollResult = NextChart.Dice.Roll(randomizer, 0);
                var Option = NextChart.GetOptionForResult(RollResult);
                var SelectedOption = NextChart.CreateSelectedOption(Option);
                Result.Add(SelectedOption);
                if (!string.IsNullOrEmpty(Option.GoToChartKey))
                {
                    NextChart = GetChart(Option.GoToChartKey);
                }
                else
                {
                    NextChart = null;
                }
            }
            return Result;
        }

        public async Task<IEnumerable<ChartCollectionModel>> LoadChartCollections(DatabaseConfiguration options)
        {
            await Task.Run(() =>
            {
                ChartCollectionBuilder builder = new ChartCollectionBuilder(_ChartSerializer);
                builder
                    .AddChartsFromResources(options)
                    .AddNameCharts(options)
                    .AddMythicCharts(options);
                ParentChartCollections = builder.BuildCollections();
                Charts = builder.BuildCharts();
            });

            return ParentChartCollections;
        }
    }
}
