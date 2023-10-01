using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.TextRepository
{
    public class CharacterDetailFlatFileSerializer : ICharacterSerializer
    {        enum State
        {
            Overview,
            Note,
            Key,
            Description,
            Range,
            EndDetail
        }

        private static readonly Dictionary<State, State> _NextState = new Dictionary<State, State>()
        {
            [State.Overview] = State.Key,
            [State.Note] = State.Key,
            [State.Key] = State.Range,
            [State.Range] = State.Description,
            [State.Description] = State.EndDetail,
            [State.EndDetail] = State.Key
        };

        private static readonly Regex _KeyPattern = new Regex("([^\\(]+)");
        private static readonly string _NotesKey = "Notes=";
        private static readonly string _NameKey = "Name=";

        public CharacterDetailFlatFileSerializer()
        {
        }

        public CharacterModel Deserialize(IChartRepository businessObject, IEnumerable<string> lines)
        {
            CharacterModel result = new CharacterModel();
            List<SelectedOption> details = new List<SelectedOption>();

            State currentState = State.Overview;
            SelectedOption currentModel = null;

            foreach (var line in lines)
            {
                (currentState, currentModel) = ProcessLine(businessObject, result, currentState, currentModel, line);
                if (currentState == State.Key && currentModel != null)
                {
                    details.Add(currentModel);
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

            builder.Append(_NameKey).Append(character.Name).Append(Environment.NewLine);
            builder.Append(_NotesKey).Append(character.Notes).Append(Environment.NewLine).Append(Environment.NewLine);

            foreach (var detail in character.Details)
            {
                SerializeToBuilder(builder, detail);
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();

        }

        private SelectedOption CreateNewDetailItem(IChartRepository businessObject, string line)
        {
            SelectedOption currentModel = new SelectedOption();
            currentModel.ChartKey = ExtractKey(line);
            currentModel.ChartName = businessObject.GetChart(currentModel.ChartKey)?.ChartName ?? string.Empty;
            return currentModel;
        }

        private string ExtractName(string line)
        {
            return line.Substring(_NameKey.Length).Trim();
        }

        private string ExtractNotes(string line)
        {
            return line.Substring(_NotesKey.Length).Trim();
        }

        private string ExtractKey(string line)
        {
            if (line.StartsWith("#"))
            {
                line = line.Substring(1);
            }

            Match match = _KeyPattern.Match(line);
            if (match.Success)
            {
                line = match.Groups[1].Value;
            }

            return line.Trim();
        }

        private bool IsName(string line)
        {
            return line.StartsWith(_NameKey);
        }

        private bool IsNote(string line)
        {
            return line.StartsWith(_NotesKey);
        }

        private (State nextState, SelectedOption model) ProcessLine(IChartRepository businessObject, CharacterModel character, State currentState, SelectedOption currentModel, string line)
        {
            State nextState = currentState;

            switch (currentState)
            {
                case State.Overview:
                    if (IsName(line))
                    {
                        character.Name = ExtractName(line);
                    }
                    else if (IsNote(line))
                    {
                        character.Notes = ExtractNotes(line);
                        nextState = State.Note;
                    }
                    else if (string.IsNullOrWhiteSpace(line))
                    {
                        nextState = State.Key;
                    }
                    break;

                case State.EndDetail:
                    nextState = State.Key;
                    break;

                case State.Note:
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        character.Notes += Environment.NewLine + line;
                    }
                    else
                    {
                        nextState = State.Key;
                    }
                    break;

                case State.Key:
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        currentModel = CreateNewDetailItem(businessObject, line);
                        nextState = _NextState[currentState];
                    }
                    break;

                case State.Description:
                    currentModel.Description = line.Trim();
                    nextState = _NextState[currentState];
                    break;

                case State.Range:
                    currentModel.Range = DiceRange.Parse(line);
                    nextState = _NextState[currentState];
                    break;
            }

            return (nextState, currentModel);
        }

        private void SerializeToBuilder(StringBuilder builder, SelectedOption detail)
        {
            // ChartKey
            builder.Append(detail.ChartKey);
            if (!string.IsNullOrEmpty(detail.ChartName))
            {
                builder.Append($" ({detail.ChartName})");
            }
            builder.Append(Environment.NewLine);

            // Range
            builder.Append(detail.Range);
            builder.Append(Environment.NewLine);

            // Description
            if (!string.IsNullOrEmpty(detail.Description))
            {
                builder.Append(detail.Description);
            }
            else
            {
                builder.Append("n/a");
            }
            builder.Append(Environment.NewLine);

        }
    }
}
