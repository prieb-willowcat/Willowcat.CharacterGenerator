using System;
using System.Diagnostics;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [DebuggerDisplay("{ChartName} - {Range} {Description}")]
    public class SelectedOption : SaveableModelBase
    {
        public string ChartKey
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public string ChartName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public string Description
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string ParentChartKey
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
        public DiceRange Range
        {
            get => GetProperty<DiceRange>();
            set => SetProperty(value);
        }

        public SelectedOption()
        {

        }

        public SelectedOption(Guid id)
        {
            Id = id;
        }
    }
}
