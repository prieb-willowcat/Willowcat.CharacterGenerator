using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Willowcat.CharacterGenerator.Model
{
    [Table("Migration")]
    [DebuggerDisplay("{Sequence}. {Name} (Ran on {DateRan})")]
    public class MigrationModel
    {
        public string Name { get; set; } = string.Empty;

        [Key]
        public int MigrationId { get; set; }

        public DateTime DateRan { get; set; }
    }
}
