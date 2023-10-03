using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Willowcat.CharacterGenerator.Application.Extension;
using Willowcat.CharacterGenerator.Core.TextRepository;
using Willowcat.CharacterGenerator.FlatFile.Repository;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.FlatFile.TextRepository
{
    public class ChartFlatFileSerializer
    {
        private static readonly Regex _NestingPattern = new Regex(@"(\w+\d+)([A-Z]+)?([a-z]+)?$");
        private static readonly string _NotesKey = "Notes:";

        enum State
        {
            Key,
            Name,
            Notes,
            Dice,
            Choices
        }

        private readonly static Dictionary<State, State> _NextState = new Dictionary<State, State>()
        {
            [State.Key] = State.Name,
            [State.Name] = State.Dice,
            [State.Dice] = State.Choices,
            [State.Choices] = State.Key
        };


        public List<FlatFileChartModel> Deserialize(string source, string text)
        {
            return Deserialize(source, text.Split('\n'));
        }

        public List<FlatFileChartModel> Deserialize(string source, IEnumerable<string> lines)
        {
            List<FlatFileChartModel> result = new();
            int index = 0;

            State currentState = State.Key;
            FlatFileChartModel currentModel = null;

            foreach (var line in lines)
            {
                (currentState, currentModel) = ProcessLine(currentState, currentModel, line);
                if (currentState == State.Name)
                {
                    currentModel.Sequence = index;
                    currentModel.Source = source;
                    if (currentModel.Dice.DiceSides == 0)
                    {
                        currentModel.Dice = GestimateDice(currentModel);
                    }
                    result.Add(currentModel);
                    index++;
                }
            }

            foreach (var chart in result)
            {
                ValidateChart(chart);
            }

            return result;
        }

        private string ExtractChartName(string line)
        {
            string result = line;
            int hashtagIndex = line.IndexOf("#");
            if (hashtagIndex > 0)
            {
                result = line.Substring(0, hashtagIndex);
            }
            return result.Trim();
        }

        private IEnumerable<string> ExtractChartTags(string line)
        {
            IEnumerable<string> result = new string[] { };
            int hashtagIndex = line.IndexOf("#");
            if (hashtagIndex > 0)
            {
                result = line
                    .Substring(hashtagIndex + 1)
                    .Split('#')
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0);
            }
            return result;
        }

        private string ExtractNotes(string line)
        {
            return line.Substring(_NotesKey.Length).Trim().Replace("\\r\\n", "\r\n");
        }

        private void ExtractKey(string line, ChartModel model)
        {
            if (line.StartsWith("#"))
            {
                int parentKeyMarkerIndex = line.IndexOf(">");
                if (parentKeyMarkerIndex > 0)
                {
                    model.Key = line.Substring(1, parentKeyMarkerIndex - 1).Trim();
                    model.ParentKey = line.Substring(parentKeyMarkerIndex + 1).Trim();
                }
                else
                {
                    model.Key = line.Substring(1).Trim();
                    Match match = _NestingPattern.Match(model.Key);
                    if (match.Success)
                    {
                        string parentKey = match.Groups[1].Value;
                        string caps = match.Groups[2].Value;
                        string roman = match.Groups[3].Value;
                        if (!string.IsNullOrEmpty(roman))
                        {
                            model.ParentKey = parentKey + (caps ?? string.Empty);
                        }
                        else if (!string.IsNullOrEmpty(caps))
                        {
                            model.ParentKey = parentKey;
                        }
                    }
                }

            }
        }

        private OptionModel ExtractOption(string chartKey, string line)
        {
            OptionModel option = null;
            string[] fields = line.Trim().Split('\t');
            if (fields.Length >= 2)
            {
                DiceRange? range = DiceRange.Parse(fields[0]);
                if (range.HasValue)
                {
                    string GoToChart = fields.Length >= 3 ? fields[2] : null;
                    option = new OptionModel(chartKey, Guid.NewGuid(), fields[1])
                    {
                        Range = range.Value,
                        GoToChartKey = GoToChart
                    };
                }
            }
            return option;
        }

        public static Dice GestimateDice(ChartModel @this)
        {
            if (@this.Options.Any())
            {
                return new Dice(1, @this.Options.Max(x => x.Range.End));
            }
            else
            {
                return new Dice();
            }
        }

        private bool IsNote(string line)
        {
            return line.StartsWith(_NotesKey);
        }

        private (State nextState, FlatFileChartModel model) ProcessLine(State currentState, FlatFileChartModel currentModel, string line)
        {
            State nextState = currentState;

            switch (currentState)
            {
                case State.Key:
                    currentModel = new FlatFileChartModel();
                    ExtractKey(line, currentModel);
                    nextState = _NextState[currentState];
                    break;

                case State.Name:
                    currentModel.ChartName = ExtractChartName(line);
                    currentModel.ParsedTags = new HashSet<string>(ExtractChartTags(line));
                    nextState = _NextState[currentState];
                    break;

                case State.Dice:
                    if (IsNote(line))
                    {
                        currentModel.Notes = ExtractNotes(line);
                    }
                    else
                    {
                        currentModel.Dice = Dice.Parse(line);
                        nextState = _NextState[currentState];
                    }
                    break;

                case State.Choices:
                    if (string.IsNullOrEmpty(line.Trim()))
                    {
                        nextState = _NextState[currentState];
                    }
                    else
                    {
                        OptionModel option = ExtractOption(currentModel.Key, line);
                        if (option != null)
                        {
                            currentModel.Options.Add(option);
                        }
                    }
                    break;
            }

            return (nextState, currentModel);
        }

        public string Serialize(IEnumerable<ChartModel> charts)
        {
            List<string> Lines = new List<string>();
            foreach (var chart in charts)
            {
                if (Lines.Any())
                {
                    Lines.Add(string.Empty);
                }
                SerializeChart(Lines, chart);
            }
            return string.Join(Environment.NewLine, Lines);
        }

        public void SerializeChart(List<string> lines, ChartModel chart)
        {
            lines.Add($"# {chart.Key}");
            lines.Add(chart.ChartName);
            lines.Add($"Notes: {chart.Notes}");
            lines.Add(chart.Dice.ToString());
            foreach (var option in chart.Options)
            {
                string Redirect = string.Empty;
                if (!string.IsNullOrEmpty(option.GoToChartKey))
                {
                    Redirect = "\t" + option.GoToChartKey;
                }
                lines.Add($"{option.Range}\t{option.Description}{Redirect}");
            }
        }

        /// <summary>
        /// Throws exception if chart is considered invalid
        /// </summary>
        /// <param name="currentModel"></param>
        private void ValidateChart(ChartModel currentModel)
        {
            if (string.IsNullOrEmpty(currentModel.Key))
            {
                throw new ChartParsingException($"{nameof(ChartModel.Key)} must have a value.", currentModel);
            }
            if (string.IsNullOrEmpty(currentModel.ChartName))
            {
                throw new ChartParsingException($"{nameof(ChartModel.ChartName)} must have a value.", currentModel);
            }
            ValidateOptions(currentModel);
        }

        /// <summary>
        /// Throws exception if chart has no options or is missing an option value
        /// </summary>
        /// <param name="chart"></param>
        private void ValidateOptions(ChartModel chart)
        {
            try
            {
                if (!chart.Options.Any())
                {
                    throw new ChartParsingException($"No valid options", chart);
                }

                int? expectedNextOption = null;
                foreach (var option in chart.Options)
                {
                    if (expectedNextOption.HasValue && expectedNextOption != option.Range.Start)
                    {
                        throw new ChartParsingException($"Missing option for dice value [{expectedNextOption}]", chart);
                    }
                    expectedNextOption = option.Range.End + 1;
                }
            }
            catch (MissingItemException ex)
            {
                throw new ChartParsingException($"Missing option for dice value [{ex.MissingItem}]", chart);
            }
            catch (Exception ex)
            {
                throw new ChartParsingException($"Error validating chart", ex, chart);
            }
        }
    }
}
