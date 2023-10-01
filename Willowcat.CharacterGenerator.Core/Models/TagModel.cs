using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [DebuggerDisplay("{Name} (Id={TagId})")]
    public class TagModel
    {
        [Key]
        public int TagId { get; set; }
        public string Name { get; set; }
    }
}
