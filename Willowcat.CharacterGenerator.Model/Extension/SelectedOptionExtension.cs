namespace Willowcat.CharacterGenerator.Model.Extension
{
    public static class SelectedOptionExtension
    {
        public static string GetSelectionText(this IEnumerable<SelectedOption> options)
        {
            return string.Join(" - ", options.Select(option => option.Description));
        }
    }
}
