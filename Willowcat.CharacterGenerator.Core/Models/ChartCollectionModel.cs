using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [Table("ChartCollection")]
    [DebuggerDisplay("{CollectionName} (Number of charts: {Charts.Count})")]
    public class ChartCollectionModel
    {
        public List<ChartModel> Charts { get; private set; } = new List<ChartModel>();
        [Key]
        public string CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string CollectionTag { get; set; }
        public string FileName { get; set; }
        public bool HideFromMainScreen { get; internal set; }
        public bool IsReadOnly { get; internal set; }
        public string ParentCollectionId { get; set; }
        public ChartCollectionModel ParentCollection { get; set; }
        public int Sequence { get; set; }
        public List<ChartCollectionModel> SubCollections { get; set; } = new List<ChartCollectionModel>();
    }
}
