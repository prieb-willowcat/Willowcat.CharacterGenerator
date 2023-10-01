using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [DebuggerDisplay("{Name} (Details Count = {Details.Count})")]
    public class CharacterModel
    {
        public IList<SelectedOption> Details { get; set; } = new List<SelectedOption>();

        public string Name { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public CharacterModel()
        {
        }

        public CharacterModel Clone()
        {
            return new CharacterModel()
            {
                Details = Details.Select(option => option.Clone()).ToList(),
                Name = Name,
                Notes = Notes,
            };
        }
    }
}
