using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.TextRepository
{
    public class CharacterPipeFileSerializer : ICharacterSerializer
    {
        private const string _NameKey = "Name";
        private const string _NotesKey = "Notes";
        private const string _OptionKey = "Option";

        private static readonly Regex _RangePattern = new Regex(@"(\-?\d+)(?:-(\-?\d+))?");

        public CharacterPipeFileSerializer()
        {
        }

        public CharacterModel Deserialize(IChartRepository businessObject, IEnumerable<string> lines)
        {
            CharacterModel result = new CharacterModel();
            List<SelectedOption> details = new List<SelectedOption>();
            bool ProcessAsNote = false;
            foreach (var line in lines)
            {
                if (line.StartsWith("#") && line.Contains("="))
                {
                    int splitIndex = line.IndexOf('=');
                    string key = line.Substring(1, splitIndex - 1);
                    string value = line.Substring(splitIndex + 1);
                    if (key.Equals(_NameKey, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Name = value.Trim();
                        ProcessAsNote = false;
                    }
                    else if (key.Equals(_NotesKey, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Notes = value.Trim();
                        ProcessAsNote = true;
                    }
                    else 
                    {
                        if (key.Equals(_OptionKey, StringComparison.OrdinalIgnoreCase))
                        {
                            var detail = ParseDetails(businessObject, value);
                            details.Add(detail);
                        }
                        ProcessAsNote = false;
                    }
                }
                else if (ProcessAsNote && !string.IsNullOrWhiteSpace(line))
                {
                    result.Notes += Environment.NewLine + line.Trim();
                }
            }
            result.Details = details;
            return result;
        }

        public CharacterModel Deserialize(IChartRepository businessObject, string text)
        {
            return Deserialize(businessObject, text.Split('\n'));
        }

        public string Serialize(CharacterModel character)
        {
            StringBuilder builder = new StringBuilder();
            AppendKeyValueLine(builder, _NameKey, character.Name);
            AppendKeyValueLine(builder, _NotesKey, character.Notes);
            builder.Append(Environment.NewLine);
            foreach (var detail in character.Details)
            {
                AppendKeyValueLine(builder, _OptionKey, GetDetailString(detail));
            }
            return builder.ToString();
        }

        public StringBuilder AppendKeyValueLine(StringBuilder builder, string key, string value)
        {
            return builder.Append($"#{key}={value}").Append(Environment.NewLine);
        }

        private DiceRange ExtractRangeOrDefault(string[] fields, int index, DiceRange defaultRange)
        {
            DiceRange result = defaultRange;
            if (index < fields.Length)
            {
                Match match = _RangePattern.Match(fields[index]);
                if (match.Success)
                {
                    int start = int.Parse(match.Groups[1].Value);

                    string endString = match.Groups[2].Value;
                    int end = !string.IsNullOrEmpty(endString) ? int.Parse(endString) : start;

                    result = new DiceRange(start, end);
                }
            }
            return result;
        }

        private string ExtractStringOrDefault(string[] fields, int index, string defaultString)
        {
            return (index < fields.Length) ? fields[index] : defaultString;
        }

        private string GetDetailString(SelectedOption detail)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(detail.ChartKey ?? string.Empty).Append("|");
            builder.Append(detail.ParentChartKey ?? string.Empty).Append("|");
            builder.Append(detail.Range).Append("|");
            builder.Append(detail.Description ?? string.Empty);
            return builder.ToString();
        }

        private SelectedOption ParseDetails(IChartRepository businessObject, string value)
        {
            string[] fields = value.Trim().Split('|');

            string chartKey       = ExtractStringOrDefault(fields, 0, string.Empty);
            string parentChartKey = ExtractStringOrDefault(fields, 1, string.Empty);
            DiceRange range       = ExtractRangeOrDefault (fields, 2, new DiceRange(0));
            string description    = ExtractStringOrDefault(fields, 3, string.Empty);
            string chartName      = businessObject.GetChart(chartKey)?.ChartName;

            if (chartName == null && chartKey.EndsWith("A"))
            {
                chartKey = chartKey.Substring(0, chartKey.Length - 1);
                chartName = businessObject.GetChart(chartKey)?.ChartName ?? string.Empty; // TODO: report error if null
            }

            return new SelectedOption()
            {
                ChartKey = chartKey,
                ChartName = chartName,
                Description = description,
                Range = range,
                ParentChartKey = string.IsNullOrEmpty(parentChartKey) ? null : parentChartKey
            };
        }
    }
}
