using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.UI.Event
{
    public class OptionSelectedEventArgs
    {
        public SelectedOption SelectedOption { get; }

        public OptionSelectedEventArgs(SelectedOption selectedOption)
        {
            SelectedOption = selectedOption;
        }
    }
}
