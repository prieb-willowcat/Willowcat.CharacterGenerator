namespace Willowcat.CharacterGenerator.Application.Interface
{
    public interface INameGenerator
    {
        bool ShowRegionSelector { get; }
        Dictionary<string, string> Regions { get; }
        IEnumerable<string> GetSavedNames(string selectedRegion);
        Task<IEnumerable<string>> GetNamesAsync(string selectedRegion);
    }
}
