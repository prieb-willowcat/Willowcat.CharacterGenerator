using System.Collections.Generic;
using System.Diagnostics;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [DebuggerDisplay("{Name} (Details Count = {Details.Count()})")]
    public class CharacterModel : SaveableModelBase
    {
        public IList<SelectedOption> Details
        {
            get => GetProperty<IList<SelectedOption>>();
            set => SetProperty(new SaveableList<SelectedOption>(value));
        }

        public string Name
        {
            get => GetProperty<string>(); 
            set => SetProperty(value);
        } 

        public string Notes
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public CharacterModel()
        {
            Details = new List<SelectedOption>();
            AcceptChanges();
        }

        public void SetSelectedOptions(IEnumerable<SelectedOption> value)
        {
            SetProperty(new SaveableList<SelectedOption>(value), nameof(Details));
        }
    }
}
