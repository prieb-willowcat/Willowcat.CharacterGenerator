using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public class FlatFileChartModel : ChartModel
    {
        [NotMapped]
        public HashSet<string> ParsedTags { get; set; } = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
    }
}
