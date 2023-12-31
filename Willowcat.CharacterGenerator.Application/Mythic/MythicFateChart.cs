﻿using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;

namespace Willowcat.CharacterGenerator.Application.Mythic
{
    public class MythicFateChart : ChartModel
    {
        private static readonly Dictionary<MythicFateOdds, Dictionary<int, int[]>> _FatePercentages = new Dictionary<MythicFateOdds, Dictionary<int, int[]>>();

        public MythicFateChart(int chaosRank)
        {
            Key = FormatChartKey(chaosRank);
            ChartName = $"Chaos Rank {chaosRank}";
            Source = "MythicRandomEventCharts";
            Sequence = 10 + chaosRank;
            SetOddsOptions(chaosRank);
        }

        public MythicFateChart(int chaosRank, MythicFateOdds odds)
        {
            Key = FormatChartKey(chaosRank, odds);
            Sequence = (int)odds;
            ParentKey = FormatChartKey(chaosRank);
            ChartName = GetDescriptionByOdds(odds);
            Source = "MythicRandomEventCharts";
            SetFatePercentageOptions(chaosRank, odds);
        }

        private string GetDescriptionByIndex(int index)
        {
            if (index == 0) return "Exception Yes";
            if (index == 1) return "Yes";
            if (index == 2) return "No";
            if (index == 3) return "Exception No";
            return "";
        }

        private string GetDescriptionByOdds(MythicFateOdds odds)
        {
            return odds switch
            {
                MythicFateOdds.Impossible => "Impossible",
                MythicFateOdds.NoWay => "No Way",
                MythicFateOdds.VeryUnlikely => "Very Unlikely",
                MythicFateOdds.Unlikely => "Unlikely",
                MythicFateOdds.EvenOdds => "50/50",
                MythicFateOdds.SomewhatLikely => "Somewhat Likely",
                MythicFateOdds.Likely => "Likely",
                MythicFateOdds.VeryLikely => "Very Likely",
                MythicFateOdds.NearSureThing => "Near Sure Thing",
                MythicFateOdds.SureThing => "A Sure Thing",
                MythicFateOdds.HasToBe => "Has To Be",
                _ => string.Empty,
            };
        }

        private OptionModel GetOddsOptionModel(int chaosRank, MythicFateOdds odds, int index)
        {
            return new OptionModel(Key, Guid.NewGuid(), GetDescriptionByOdds(odds))
            {
                Range = new DiceRange(index, index),
                GoToChartKey = FormatChartKey(chaosRank, odds)
            };
        }

        private string FormatChartKey(int chaosRank, MythicFateOdds? odds = null)
        {
            return odds.HasValue ? $"MythicFate_{chaosRank}_{odds}" : $"MythicFate_{chaosRank}";
        }

        private static void LoadFatePercentages()
        {
            string[] lines = Properties.Resources.MythicFateChart.Split('\n');
            foreach (string line in lines)
            {
                string[] fields = line.Trim().Split('\t');
                if (fields.Length <= 1) continue;

                MythicFateOdds odds = (MythicFateOdds)Enum.Parse(typeof(MythicFateOdds), fields[0]);

                Dictionary<int, int[]> percentagesByChaosRank = new Dictionary<int, int[]>();
                int chaosRank = 9;
                for (int i = 1; i < fields.Length; i++)
                {
                    int[] percentages = fields[i].Split(' ').Select(x => int.Parse(x)).ToArray();
                    percentagesByChaosRank[chaosRank] = percentages;
                    chaosRank--;
                }
                _FatePercentages[odds] = percentagesByChaosRank;
            }
        }

        private void SetFatePercentageOptions(int chaosRank, MythicFateOdds odds)
        {
            if (_FatePercentages.Count == 0)
            {
                LoadFatePercentages();
            }
            Dice = new Dice(1, 100);
            Options.Clear();
            int start = 1;
            int index = 0;
            foreach (var percentage in _FatePercentages[odds][chaosRank])
            {
                if (percentage > 0)
                {
                    int end = percentage;
                    if (index == 2)
                    {
                        end = percentage - 1;
                    }
                    if (end > 100)
                    {
                        end = 100;
                    }
                    DiceRange range = new DiceRange(start, end);
                    this.AddOption(start, end, GetDescriptionByIndex(index));
                    start = end + 1;
                }
                index++;
            }
            this.AddOption(start, 100, GetDescriptionByIndex(index));
        }

        private void SetOddsOptions(int chaosRank)
        {
            Dice = new Dice(1, 11);
            Options.Clear();

            int index = 1;
            foreach (var odds in Enum.GetValues(typeof(MythicFateOdds)))
            {
                Options.Add(GetOddsOptionModel(chaosRank, (MythicFateOdds)odds, index));
                index++;
            }
        }

    }
}
