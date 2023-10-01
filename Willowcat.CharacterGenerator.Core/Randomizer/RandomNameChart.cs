using System.Collections.Generic;
using Willowcat.CharacterGenerator.Core.Randomizer;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public class RandomNameChart : ChartModel
    {
        public INameGenerator NameGenerator { get; private set; } = null;

        public Dictionary<string, string> Regions { get; private set; } = new Dictionary<string, string>();

        public bool ShowRegionSelector { get; protected set; } = false;

        protected RandomNameChart(NameCategory nameCategory, string apiKey = null)
        {
            switch (nameCategory)
            {
                case NameCategory.Elvish:
                    NameGenerator = new RandomElvenNames();
                    ShowRegionSelector = false;
                    break;

                case NameCategory.Human_Female:
                    NameGenerator = new RandomBehindTheName(apiKey, Gender.Female, 18);
                    ShowRegionSelector = true;
                    break;

                case NameCategory.Human_Male:
                    NameGenerator = new RandomBehindTheName(apiKey, Gender.Male, 18);
                    ShowRegionSelector = true;
                    break;
            }

            if (ShowRegionSelector)
            {
                foreach (var kvp in RandomBehindTheName.Regions)
                {
                    Regions[kvp.Key] = kvp.Value;
                }
            }

            //ParsedTags = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
            //{
            //    "Names"
            //};
            Source = "Names";
        }

        public static RandomNameChart GetElvenNameChart()
        {
            return new RandomNameChart(NameCategory.Elvish)
            {
                Key = "names-elven",
                ChartName = "Elf Names"
            };
        }

        public static RandomNameChart GetFemaleNameChart(string apiKey)
        {
            return new RandomNameChart(NameCategory.Human_Female, apiKey)
            {
                Key = "names-female",
                ChartName = "Female Names"
            };
        }

        public static RandomNameChart GetMaleNameChart(string apiKey)
        {
            return new RandomNameChart(NameCategory.Human_Male, apiKey)
            {
                Key = "names-male",
                ChartName = "Male Names"
            };
        }
    }
}
