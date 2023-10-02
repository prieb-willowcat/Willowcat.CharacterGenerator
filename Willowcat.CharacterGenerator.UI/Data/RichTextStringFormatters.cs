using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.UI.Data
{
    public class RichTextStringFormatters
    {
        private readonly static Regex _ChartLinkFinder = new Regex(@"\[#([\w\s]+)\]");

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
                paragraph.Inlines.Add(inline);
            }

            FlowDocument document = new FlowDocument();
            document.Blocks.Add(paragraph);
            return document;
        }

        public static List<TextRun> BuildTextRunListFromDescription(ChartService chartService, string text)
        {
            List<TextRun> result = new List<TextRun>();
            if (!string.IsNullOrEmpty(text))
            {
                MatchCollection matches = _ChartLinkFinder.Matches(text);
                int i = 0;
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        string chartKey = match.Groups[1].Value;

                        ChartModel matchingChart = chartService.GetChart(chartKey);
                        string label = matchingChart?.ChartName ?? chartKey;

                        int length = match.Index - i;

                        result.Add(new TextRun(text.Substring(i, length)));
                        result.Add(new TextRun(label, chartKey));

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

        public static string ReplaceChartKeysWithChartNames(ChartService chartService, string text)
        {
            var textRuns = BuildTextRunListFromDescription(chartService, text);

            StringBuilder builder = new StringBuilder();
            foreach (var textRun in textRuns)
            {
                builder.Append(textRun.Text);
            }

            return builder.ToString();
        }
    }

    public class TextRun
    {
        public string Text { get; set; }
        public bool IsLinkToChart => !string.IsNullOrEmpty(ChartKey);
        public string ChartKey { get; set; }

        public TextRun(string text)
        {
            Text = text;
        }

        public TextRun(string text, string chartKey)
        {
            Text = text;
            ChartKey = chartKey;
        }
    }
}
