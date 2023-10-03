using Willowcat.CharacterGenerator.Application.Interface;

namespace Willowcat.CharacterGenerator.OnlineGenerators.Generator
{
    public class RandomElvenNames : INameGenerator
    {
        public readonly IHttpJsonClient _webClient;
        private readonly string _BaseUrl = "https://www.namegenerator.biz/application/p.php?type=4&id=elven_female_names&id2=elven_male_names&spaceflag=false";
        private readonly string _TempDirectory = "CharacterGeneration";
        private readonly string _TempFileName = "elven_names.txt";

        public RandomElvenNames(IHttpJsonClient webClient)
        {
            _webClient = webClient;
        }

        public bool ShowRegionSelector => false;

        private IEnumerable<string> GetElvenNames(string tempFilePath)
        {
            string nextName = string.Empty;
            string response = _webClient.DownloadJson(_BaseUrl);
            string[] names = response.Split(new char[] { ',' });
            SaveNames(tempFilePath, names);
            return names.ToList();
        }

        public Task<IEnumerable<string>> GetNamesAsync(string selectedRegion)
        {
            string path = Path.Combine(Path.GetTempPath(), _TempDirectory, _TempFileName);
            return Task.Run(() => GetElvenNames(path));
        }

        public IEnumerable<string> GetSavedNames(string selectedRegion)
        {
            string path = Path.Combine(Path.GetTempPath(), _TempDirectory, _TempFileName);
            return GetSavedElvenNames(path);
        }

        private IEnumerable<string> GetSavedElvenNames(string tempFilePath)
        {
            if (File.Exists(tempFilePath))
            {
                return File.ReadLines(tempFilePath);
            }
            else
            {
                return new string[] { };
            }
        }

        private void SaveNames(string tempFilePath, string[] names)
        {
            string parentDirectory = Path.GetDirectoryName(tempFilePath);
            if (!Directory.Exists(parentDirectory))
            {
                Directory.CreateDirectory(parentDirectory);
            }
            File.WriteAllLines(tempFilePath, names);
        }
    }
}
