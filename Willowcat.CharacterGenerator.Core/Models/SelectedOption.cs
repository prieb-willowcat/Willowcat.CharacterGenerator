using System;
using System.Diagnostics;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [DebuggerDisplay("{ChartName} - {Range} {Description}")]
    public class SelectedOption
    {
        public string ChartKey { get; set; } = string.Empty;
        public string ChartName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string ParentChartKey { get; set; } = string.Empty;
        public DiceRange Range { get; set; } = new DiceRange();

        public SelectedOption()
        {

        }

        public SelectedOption(Guid id)
        {
            Id = id;
        }

        public SelectedOption Clone()
        {
            return new SelectedOption()
            {
                ChartKey = ChartKey,
                ChartName = ChartName,
                Description = Description,
                Id = Id,
                ParentChartKey = ParentChartKey,
                Range = Range
            };
        }
    }
}
