using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Willowcat.CharacterGenerator.Core.Randomizer
{
    public class RandomBehindTheName : INameGenerator
    {
        /// <summary>
        /// See: https://www.behindthename.com/api/help.php
        /// </summary>
        private const int _MaxNumberToSelect = 6;
        private const string _BaseUrl = "https://www.behindthename.com/api/random.json?";
        private const string _TempDirectory = "CharacterGeneration";

        private readonly Gender _SelectedGender = Gender.Random;
        private readonly int _NumberToSelect = 6;
        private readonly string _ApiKey = string.Empty;
        private readonly WebClientWrapper _WebClient = null;

        /// <summary>
        /// https://www.behindthename.com/api/appendix2.php
        /// </summary>
        public static Dictionary<string, string> Regions = new Dictionary<string, string>()
        {
            ["afr"] = "African",
            ["alb"] = "Albanian",
            ["ara"] = "Arabic",
            ["arm"] = "Armenian",
            ["aze"] = "Azerbaijani",
            ["bas"] = "Basque",
            ["bre"] = "Breton",
            ["bul"] = "Bulgarian",
            ["cat"] = "Catalan",
            ["chi"] = "Chinese",
            ["cro"] = "Croatian",
            ["cze"] = "Czech",
            ["dan"] = "Danish",
            ["dut"] = "Dutch",
            ["eng"] = "English",
            ["esp"] = "Esperanto",
            ["est"] = "Estonian",
            ["fin"] = "Finnish",
            ["fre"] = "French",
            ["fri"] = "Frisian",
            ["gal"] = "Galician",
            ["geo"] = "Georgian",
            ["ger"] = "German",
            ["gre"] = "Greek",
            ["haw"] = "Hawaiian",
            ["heb"] = "Hebrew",
            ["hun"] = "Hungarian",
            ["ice"] = "Icelandic",
            ["igb"] = "Igbo",
            ["ind"] = "Indian",
            ["ins"] = "Indonesian",
            ["iri"] = "Irish",
            ["ita"] = "Italian",
            ["jap"] = "Japanese",
            ["kaz"] = "Kazakh",
            ["khm"] = "Khmer",
            ["kor"] = "Korean",
            ["lat"] = "Latvian",
            ["lim"] = "Limburgish",
            ["lth"] = "Lithuanian",
            ["mac"] = "Macedonian",
            ["mao"] = "Maori",
            ["ame"] = "Native American",
            ["nor"] = "Norwegian",
            ["occ"] = "Occitan",
            ["per"] = "Persian",
            ["pol"] = "Polish",
            ["por"] = "Portuguese",
            ["rmn"] = "Romanian",
            ["rus"] = "Russian",
            ["sco"] = "Scottish",
            ["ser"] = "Serbian",
            ["slk"] = "Slovak",
            ["sln"] = "Slovene",
            ["spa"] = "Spanish",
            ["swe"] = "Swedish",
            ["tha"] = "Thai",
            ["tur"] = "Turkish",
            ["ukr"] = "Ukrainian",
            ["urd"] = "Urdu",
            ["vie"] = "Vietnamese",
            ["wel"] = "Welsh",
            ["yor"] = "Yoruba"
        };

        public RandomBehindTheName(string apiKey, Gender gender, int numberToSelect, WebClientWrapper webClient = null)
        {
            _ApiKey = apiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("valid API key required for behindthename.com", nameof(apiKey));
            }
            _NumberToSelect = numberToSelect;
            _SelectedGender = gender;
            _WebClient = webClient ?? new WebClientWrapper();
        }

        /// <summary>
        /// Limited to 6 names at a time.
        /// https://www.behindthename.com/api/random.json?usage=ita&gender=f&key=#key#
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="gender"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        private static string BuildUrl(string apiKey, Gender gender, string region)
        {
            StringBuilder urlBuilder = new StringBuilder(_BaseUrl);

            urlBuilder.Append($"key={apiKey}");

            if (!string.IsNullOrEmpty(region))
            {
                urlBuilder.Append($"&usage={region}");
            }

            if (gender != Gender.Random)
            {
                string genderValue = "";
                switch (gender)
                {
                    case Gender.Male: genderValue = "m"; break;
                    case Gender.Female: genderValue = "f"; break;
                }
                urlBuilder.Append($"&gender={genderValue}");
            }

            return urlBuilder.ToString();
        }

        public async Task<IEnumerable<string>> GetNamesAsync(string selectedRegion)
        {
            string url = BuildUrl(_ApiKey, _SelectedGender, selectedRegion);
            List<string> names = new List<string>();
            while (names.Count < _NumberToSelect)
            {
                // rate-limited to 2 per second
                await Task.Delay(500);
                int numberLeftToRequest = _NumberToSelect - names.Count;
                int subsetToSelect = numberLeftToRequest > _MaxNumberToSelect ? _MaxNumberToSelect : numberLeftToRequest;
                var responseNames = await Task.Run(() => GetNamesFromApi(url, subsetToSelect));
                names.AddRange(responseNames);
            }
            SaveNames(selectedRegion, names);
            return names;
        }

        /// <summary>
        /// {"names":["Severina","Luciana"]}
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IEnumerable<string> GetNamesFromApi(string url, int numberToSelect)
        {
            string fullUrl = url + $"&number={numberToSelect}";
            string jsonResponse = _WebClient.Download(fullUrl);
            var response = JsonSerializer.Deserialize<BehindTheNamesRandomResponse>(jsonResponse);
            if (response != null)
            {
                return response.names;
            }
            else
            {
                return new string[] { };
            }
        }

        public IEnumerable<string> GetSavedNames(string selectedRegion)
        {
            var names = new string[] { };
            string fileName = $"behindthenames-{_SelectedGender}-{selectedRegion ?? string.Empty}.txt";
            string path = Path.Combine(Path.GetTempPath(), _TempDirectory, fileName);
            if (File.Exists(path))
            {
                names = File.ReadAllLines(path);
            }
            return names;
        }

        private void SaveNames(string selectedRegion, List<string> names)
        {
            string fileName = $"behindthenames-{_SelectedGender}-{selectedRegion ?? string.Empty}.txt";
            string path = Path.Combine(Path.GetTempPath(), _TempDirectory, fileName);
            File.WriteAllLines(path, names.ToArray());
        } 
    }

    internal class BehindTheNamesRandomResponse
    {
        public string[] names { get; set; }
    }
}
