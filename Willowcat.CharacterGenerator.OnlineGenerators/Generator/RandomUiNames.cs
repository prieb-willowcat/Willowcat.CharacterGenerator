﻿using System.Text;
using System.Text.Json;
using Willowcat.CharacterGenerator.BehindTheName.Generator.Json;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Randomizer;

namespace Willowcat.CharacterGenerator.BehindTheName.Generator
{

    public class RandomUiNames
    {

        private const int _DefaultAmount = 25;
        private const int _MaximumAmount = 500;
        private const int _MinimumAmount = 1;
        private const string _BaseUrl = "https://uinames.com/api/";

        public static Dictionary<string, string> Regions = new Dictionary<string, string>()
        {
            ["All"] = "all",
            ["Albania"] = "albania",
            ["Argentina"] = "argentina",
            ["Austria"] = "austria",
            ["Brazil"] = "brazil",
            ["China"] = "china",
            ["Egypt"] = "egypt",
            ["England"] = "england",
            ["Germany"] = "germany",
            ["Finland"] = "finland",
            ["France"] = "france",
            ["India"] = "india",
            ["Iraq"] = "iraq",
            ["Iran"] = "iran",
            ["Italy"] = "italy",
            ["Japan"] = "japan",
            ["Mexico"] = "mexico",
            ["Nigeria"] = "nigeria",
            ["Pakistan"] = "pakistan",
            ["Russia"] = "russia",
            ["Spain"] = "spain",
            ["Saudi Arabia"] = "saudi+arabia",
            ["Turkey"] = "turkey",
            ["United States"] = "united+states"
        };

        private readonly List<UiName> _RandomEarthNames = new List<UiName>();
        private readonly IHttpJsonClient _UiNamesWebClient;

        public RandomUiNames(IHttpJsonClient webClient)
        {
            _UiNamesWebClient = webClient;
        }

        private string BuildUrl(Gender gender, string region)
        {
            StringBuilder urlBuilder = new StringBuilder(_BaseUrl);
            urlBuilder.Append($"?amount={_DefaultAmount}");

            if (!string.IsNullOrEmpty(region))
            {
                urlBuilder.Append($"&region={region}");
            }

            string genderValue = "";
            switch (gender)
            {
                case Gender.Male: genderValue = "male"; break;
                case Gender.Female: genderValue = "female"; break;
                case Gender.Random: genderValue = "random"; break;
            }
            urlBuilder.Append($"&gender={genderValue}");

            return urlBuilder.ToString();
        }

        public string NextHumanName(Gender gender = Gender.Random, string? region = null)
        {
            if (!_RandomEarthNames.Any(n => n.IsMatch(gender, region)))
            {
                var url = BuildUrl(gender, region);
                var json = _UiNamesWebClient.DownloadJson(url);
                Console.WriteLine("JSON: " + json);
                var names = JsonSerializer.Deserialize<UiName[]>(json);
                _RandomEarthNames.AddRange(names);
            }

            string result = _RandomEarthNames.First(n => n.IsMatch(gender, region)).FullName;
            _RandomEarthNames.RemoveAt(0);
            return result;
        }

        public List<string> NextHumanNames(int count, Gender gender = Gender.Random, string region = null)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < count; i++)
            {
                names.Add(NextHumanName(gender, region));
            }
            return names;
        }
    }
}