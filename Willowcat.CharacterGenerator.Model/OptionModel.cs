using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Willowcat.CharacterGenerator.Model
{
    [Table("ChartOption")]
    [DebuggerDisplay("{Range} {Description} ({ChartKey})")]
    public class OptionModel
    {
        public ChartModel? Chart { get; set; }

        public string ChartKey { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ChartModel? GoToChart { get; set; }

        public string? GoToChartKey { get; set; } = string.Empty;

        [Key]
        public Guid OptionId { get; set; } = Guid.Empty;//TODO: change OptionId to int

        public DiceRange Range { get; set; } = new DiceRange();

        public string GetRangeString() => Range.ToString();

        public OptionModel()
        {
        }

        public OptionModel(string chartKey, Guid id, string description, DiceRange range = default)
        {
            ChartKey = chartKey;
            OptionId = id;
            Description = description;
            Range = range;
        }
    }
}
