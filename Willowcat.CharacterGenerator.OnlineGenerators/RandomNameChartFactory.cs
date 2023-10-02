using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.BehindTheName.Generator;
using Willowcat.CharacterGenerator.Core.Randomizer;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public class RandomNameChartFactory
    {
        private readonly IServiceProvider _provider;
        private readonly Func<string> _getBehindTheNameApiKey;

        public RandomNameChartFactory(IServiceProvider provider, Func<string> getBehindTheNameApiKey)
        {
            _provider = provider;
            _getBehindTheNameApiKey = getBehindTheNameApiKey;
        }

        public RandomNameChart? GetChart(NameCategory nameCategory)
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

                if (chart != null && nameGenerator.ShowRegionSelector)
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
