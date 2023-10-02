namespace Willowcat.CharacterGenerator.Core
{
    public class DatabaseConfiguration
    {
        public string BehindTheNameApiKey { get; set; }
        public bool CanLoadBehindTheNameCharts => !string.IsNullOrEmpty(BehindTheNameApiKey);
    }
}
