﻿using Willowcat.CharacterGenerator.Core.Randomizer;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Models
{

    public class RandomNameChart : ChartModel
    {
        public RandomNameChart(INameGenerator nameGenerator, string key, string name)
        {
            NameGenerator = nameGenerator;
            Key = key;
            ChartName = name;
            Source = "Names";
        }

        public INameGenerator NameGenerator { get; private set; }

        public Dictionary<string, string> Regions { get; private set; } = new Dictionary<string, string>();

        public bool ShowRegionSelector { get; protected set; } = false;
    }
}