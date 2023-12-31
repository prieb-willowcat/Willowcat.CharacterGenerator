﻿using Microsoft.EntityFrameworkCore;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.EntityFramework.Database;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.EntityFramework.Repository
{
    public class ChartService : IChartRepository
    {
        private readonly ChartContextFactory _contextFactory;
        private readonly IEnumerable<IAutoGeneratorFactory> _autoGeneratorFactories;

        public ChartService(ChartContextFactory contextFactory, IEnumerable<IAutoGeneratorFactory> autoGeneratorFactories)
        {
            _contextFactory = contextFactory;
            _autoGeneratorFactories = autoGeneratorFactories;
        }

        public ChartModel? GetChart(string chartKey)
        {
            ChartModel? model = null;
            if (!string.IsNullOrEmpty(chartKey))
            {
                using (ChartContext context = _contextFactory.GetChartContext())
                {
                    model = context.Charts.Find(chartKey);
                    if (model != null)
                    {
                        context.Entry(model)
                            .Collection(x => x.Options)
                            .Load();
                    }
                }

                if (model != null && model.AutogenerateOptions)
                {
                    foreach (var autoGeneratorFactory in _autoGeneratorFactories)
                    {
                        if (autoGeneratorFactory.CanAutoGenerate(model))
                        {
                            model = autoGeneratorFactory.GetAutoGeneratingChart(model);
                        }
                    }
                }
            }
            return model;
        }

        public ChartCollectionModel? GetCollection(string source)
        {
            ChartCollectionModel? model = null;
            if (!string.IsNullOrEmpty(source))
            {
                using (ChartContext context = _contextFactory.GetChartContext())
                {
                    model = context.ChartCollections.Find(source);
                }
            }
            return model;
        }

        public List<string> GetChartHierarchyPath(ChartModel chart)
        {
            List<string> path = new();
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

            using (ChartContext context = _contextFactory.GetChartContext())
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
                .Include(x => x.Charts.Where(ch => ch.ParentKey == null).OrderBy(x => x.Sequence))
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
                    .OrderBy(x => x.Sequence)
                    .ToListAsync();
            }
        }
    }
}
