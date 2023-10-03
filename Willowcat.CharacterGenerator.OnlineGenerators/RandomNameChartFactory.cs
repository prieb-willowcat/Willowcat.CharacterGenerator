﻿using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.BehindTheName.Generator;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.OnlineGenerators.Generator;

namespace Willowcat.CharacterGenerator.OnlineGenerators
{
    public class RandomNameChartFactory : IChartCollectionRepository, IAutoGeneratorFactory
    {
        private readonly IServiceProvider _provider;
        private readonly Func<string> _getBehindTheNameApiKey;

        public RandomNameChartFactory(IServiceProvider provider, Func<string> getBehindTheNameApiKey)
        {
            _provider = provider;
            _getBehindTheNameApiKey = getBehindTheNameApiKey;
        }

        public Task<IEnumerable<ChartModel>> BuildChartsAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<ChartModel> charts = new List<ChartModel>()
            {
                GetChart(NameCategory.Elvish),
                GetChart(NameCategory.Human_Female),
                GetChart(NameCategory.Human_Male)
            };

            return Task.FromResult(charts);
        }

        public Task<IEnumerable<ChartCollectionModel>> BuildCollectionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IEnumerable<ChartCollectionModel>)Array.Empty<ChartCollectionModel>());
        }

        public bool CanAutoGenerate(ChartModel chart) => chart.Source == "Names";

        public ChartModel GetAutoGeneratingChart(ChartModel chart)
        {
            if (CanAutoGenerate(chart))
            {
                switch (chart.Key)
                {
                    case "names-elven": return GetChart(NameCategory.Elvish, true);
                    case "names-female": return GetChart(NameCategory.Human_Female, true);
                    case "names-male": return GetChart(NameCategory.Human_Male, true);
                }
            }

            return chart;
        }

        public RandomNameChart GetChart(NameCategory nameCategory, bool loadRegions = false)
        {
            INameGenerator? nameGenerator = GetNameGenerator(nameCategory);
            RandomNameChart? chart = null;
            if (nameGenerator != null)
            {
                switch (nameCategory)
                {
                    case NameCategory.Elvish:
                        chart = new RandomNameChart(nameGenerator, "names-elven", "Elf Names");
                        break;

                    case NameCategory.Human_Female:
                        chart = new RandomNameChart(nameGenerator, "names-female", "Female Names");
                        break;

                    case NameCategory.Human_Male:
                        chart = new RandomNameChart(nameGenerator, "names-male", "Male Names");
                        break;
                }

                if (chart != null && nameGenerator.ShowRegionSelector && loadRegions)
                {
                    foreach (var kvp in RandomBehindTheName.Regions)
                    {
                        chart.Regions[kvp.Key] = kvp.Value;
                    }
                }
            }
            return chart;
        }

        public INameGenerator? GetNameGenerator(NameCategory nameCategory)
        {
            string? key = _getBehindTheNameApiKey != null ? _getBehindTheNameApiKey() : null;
            var httpClient = _provider.GetRequiredService<IHttpJsonClient>();
            return nameCategory switch
            {
                NameCategory.Elvish => new RandomElvenNames(httpClient),
                NameCategory.Human_Female => new RandomBehindTheName(httpClient, key, Gender.Female, 18),
                NameCategory.Human_Male => new RandomBehindTheName(httpClient, key, Gender.Male, 18),
                _ => null
            };
        }
    }
}
