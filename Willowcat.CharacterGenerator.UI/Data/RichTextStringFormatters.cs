using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Willowcat.CharacterGenerator.EntityFramework.Repository;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;
using Willowcat.CharacterGenerator.UI.View.Controls;

namespace Willowcat.CharacterGenerator.UI.Data
{
    public class RichTextStringFormatters
    {
        private readonly static Regex _linkFinder = new Regex(@"(\[#(?<key>[\w\s]+)\]|\{(?<ct>\d*)d(?<sz>\d+)(?: ?(?<op>[+\-\*\/]) ?(?<mod>\d+))?\})");

        public static FlowDocument AddLocalLinksToChartKeysDocument(ChartService chartService, string text, ICommand chartHyperLinkCommand)
        {
            var textRuns = BuildTextRunListFromDescription(chartService, text);

            Paragraph paragraph = new Paragraph();
            foreach (var textRun in textRuns)
            {
                Inline inline = new Run(textRun.Text);
                if (textRun.IsLinkToChart)
                {
                    inline = new Hyperlink(inline)
                    {
                        Command = chartHyperLinkCommand,
                        CommandParameter = textRun.ChartKey,
                    };
                }
                else if (textRun.Dice.HasValue)
                {
                    inline = new DiceRollerHyperlink(inline, textRun.Dice.Value, textRun.Operator, textRun.Modifier);
                }
                else
                {
                    inline = new Run(textRun.Text ?? string.Empty);
                }
                paragraph.Inlines.Add(inline);
            }

            FlowDocument document = new FlowDocument();
            document.Blocks.Add(paragraph);
            return document;
        }

        public static string AddLinks(ChartService chartService, string text)
        {
            var textRuns = BuildTextRunListFromDescription(chartService, text);

            StringBuilder builder = new StringBuilder();
            foreach (var textRun in textRuns)
            {
                builder.Append(textRun.Text);
            }

            return builder.ToString();
        }

        private static TextRun CreateChartTextRun(ChartService chartService, string chartKey)
        {
            ChartModel matchingChart = chartService.GetChart(chartKey);
            var run = new TextRun(matchingChart?.ChartName ?? chartKey, chartKey);
            return run;
        }

        private static TextRun CreateDiceTextRun(string countString, string @operator, string modifierString, int size)
        {
            var diceCount = 1;
            if (!string.IsNullOrEmpty(countString))
            {
                diceCount = int.Parse(countString);
                if (diceCount <= 0) diceCount = 1;
            }
            var modifier = 1;
            if (!string.IsNullOrEmpty(modifierString))
            {
                modifier = int.Parse(modifierString);
            }
            var dice = new Dice(diceCount, size);
            var run = new TextRun(dice, @operator, modifier);
            return run;
        }

        public static List<TextRun> BuildTextRunListFromDescription(ChartService chartService, string text)
        {
            List<TextRun> result = new List<TextRun>();
            if (!string.IsNullOrEmpty(text))
            {
                MatchCollection matches = _linkFinder.Matches(text);
                int i = 0;
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        int length = match.Index - i;
                        result.Add(new TextRun(text.Substring(i, length)));

                        string chartKey = match.Groups["key"].Value;
                        if (!string.IsNullOrEmpty(chartKey))
                        {
                            result.Add(CreateChartTextRun(chartService, chartKey));
                        }

                        string countString = match.Groups["ct"].Value;
                        string sizeString = match.Groups["sz"].Value;
                        string @operator = match.Groups["op"].Value;
                        string modifierString = match.Groups["mod"].Value;
                        if (!string.IsNullOrEmpty(sizeString) && int.TryParse(sizeString, out int size))
                        {
                            result.Add(CreateDiceTextRun(countString, @operator, modifierString, size));
                        }

                        i = match.Index + match.Length;
                    }
                }

                if (i < text.Length)
                {
                    result.Add(new TextRun(text.Substring(i)));
                }
            }
            return result;
        }
    }

    public class TextRun
    {
        public string ChartKey { get; set; }
        public Dice? Dice { get; }
        public bool IsLinkToChart => !string.IsNullOrEmpty(ChartKey);
        public int Modifier { get; }
        public string Operator { get; }
        public string Text { get; set; }

        public TextRun(string text)
        {
            Text = text;
        }

        public TextRun(string text, string chartKey)
        {
            Text = text;
            ChartKey = chartKey;
        }

        public TextRun(Dice dice, string op, int modifier)
        {
            Dice = dice;
            Operator = op;
            Modifier = modifier;
            Text = dice.FormatDice(Operator, modifier);
        }
    }
}
