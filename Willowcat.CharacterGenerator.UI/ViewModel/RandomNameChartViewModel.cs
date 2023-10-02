using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class RandomNameChartViewModel : ChartViewModel
    {
        private readonly RandomNameChart _randomNameChart;

        private ObservableCollection<RegionOption> _regionOptions = new();
        private RegionOption _selectedRegion = null;

        public RandomNameChartViewModel(RandomNameChart chart, Random randomizer, ChartService chartService, IEventAggregator eventAggregator) 
            : base(chart, randomizer, chartService, eventAggregator)
        {
            _randomNameChart = chart;
        }

        public override bool CanDynamicallyGenerateOptions => true;

        public override ObservableCollection<RegionOption> RegionOptions
        {
            get => _regionOptions; 
            set
            {
                _regionOptions = value;
                OnPropertyChanged();
            }
        }

        public override RegionOption SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                _selectedRegion = value;
                if (value != null)
                {
                    Properties.Settings.Default.LastRegionSelected = value.Key;
                    Properties.Settings.Default.Save();
                    LoadSavedOptions(value.Key);
                }
                OnPropertyChanged();
            }
        }

        public override bool ShowRegionSelector => _randomNameChart.ShowRegionSelector;

        public override void Initialize(int? selectedRange)
        {
            ObservableCollection<RegionOption> newRegionOptions = new ObservableCollection<RegionOption>();
            RegionOption selectedRegionOption = null;

            if (_randomNameChart.Regions != null)
            {
                string lastRegionSelected = Properties.Settings.Default.LastRegionSelected;

                foreach (var kvp in _randomNameChart.Regions)
                {
                    var option = new RegionOption(kvp.Key, kvp.Value);
                    newRegionOptions.Add(option);
                    if (kvp.Key == lastRegionSelected)
                    {
                        selectedRegionOption = option;
                    }
                }
                LoadSavedOptions(SelectedRegion?.Key);
            }

            RegionOptions = newRegionOptions;
            SelectedRegion = selectedRegionOption;
            InitializeOptions(selectedRange);
        }

        protected override Task GenerateOptionsAsync(Random randomizer)
        {
            return GenerateOptionsAsync(randomizer, _selectedRegion?.Key);
        }

        public async Task GenerateOptionsAsync(Random randomizer, string selection)
        {
            if (_randomNameChart.NameGenerator != null)
            {
                var names = await _randomNameChart.NameGenerator.GetNamesAsync(selection);
                LoadOptions(names);
            }
        }

        public void LoadSavedOptions(string selection = null)
        {
            if (_randomNameChart.NameGenerator != null)
            {
                var names = _randomNameChart.NameGenerator.GetSavedNames(selection);
                LoadOptions(names);
            }
        }

        private void LoadOptions(IEnumerable<string> names)
        {
            _randomNameChart.Options.Clear();
            int i = 0;
            foreach (var name in names)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    i++;
                    _randomNameChart.AddOption(i, i, name);
                }
            }

            if (i > 0)
            {
                _randomNameChart.Dice = new Dice(1, i);
            }
            else
            {
                _randomNameChart.Dice = new Dice(1, 10);
            }
            OnPropertyChanged(nameof(ChartDice));
        }
    }
}
