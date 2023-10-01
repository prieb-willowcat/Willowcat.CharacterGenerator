using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [DebuggerDisplay("{ChartName} {Dice} ({Key})")]
    [Table("Chart")]
    public class ChartModel
    {        
        public string ChartName { get; set; }
        
        public Dice Dice { get; set; } = new Dice(0, 0);
        
        [Key]
        public string Key { get; set; }
        
        public string Notes { get; set; }

        public List<OptionModel> Options { get; set; } = new List<OptionModel>();

        public ChartModel ParentChart { get; set; }

        public string ParentKey { get; set; }

        public int Sequence { get; set; }
        
        public string Source { get; set; }
        
        public List<ChartModel> SubCharts { get; set; } = new List<ChartModel>();

        public List<TagModel> Tags { get; set; } = new List<TagModel>();
    }
}
