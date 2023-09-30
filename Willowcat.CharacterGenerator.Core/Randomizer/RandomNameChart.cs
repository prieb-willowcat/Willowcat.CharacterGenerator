using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Core.Randomizer;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public class RandomNameChart : ChartModel
    {
        private readonly INameGenerator _NameGenerator = null;

        public override bool CanDynamicallyGenerateOptions { get; protected set; } = true;

        public override bool ShowRegionSelector { get; protected set; } = true;

        protected RandomNameChart(NameCategory nameCategory, string apiKey = null)
        {
            switch (nameCategory)
            {
                case NameCategory.Elvish:
                    _NameGenerator = new RandomElvenNames();
                    ShowRegionSelector = false;
                    break;

                case NameCategory.Human_Female:
                    _NameGenerator = new RandomBehindTheName(apiKey, Gender.Female, 18);
                    ShowRegionSelector = true;
                    break;

                case NameCategory.Human_Male:
                    _NameGenerator = new RandomBehindTheName(apiKey, Gender.Male, 18);
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

            ParsedTags = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
            {
                "Names"
            };
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

        public override async Task GenerateOptionsAsync(Random randomizer, string selection)
        {
            if (_NameGenerator != null)
            {
                var names = await _NameGenerator.GetNamesAsync(selection);
                LoadOptions(names);
            }
        }

        public override void LoadSavedOptions(string selection = null)
        {
            if (_NameGenerator != null)
            {
                var names = _NameGenerator.GetSavedNames(selection);
                LoadOptions(names);
            }
        }

        private void LoadOptions(IEnumerable<string> names)
        {
            ClearOptions();
            int i = 0;
            foreach (var name in names)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    i++;
                    AddOption(i, i, name);
                }
            }

            if (i > 0)
            {
                Dice = new Dice(1, i);
            }
            else
            {
                Dice = new Dice(1, 10);
            }
        }
    }
}
