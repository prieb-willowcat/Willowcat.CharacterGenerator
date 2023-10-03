using System;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.FlatFile.TextRepository
{
    public class ChartParsingException : Exception
    {
        public ChartModel ChartModel { get; private set; }

        public ChartParsingException(string message, Exception innerException, ChartModel parsedChart)
            : base(message, innerException)
        {
            ChartModel = parsedChart;
        }

        public ChartParsingException(string message, ChartModel parsedChart)
            : base(message)
        {
            ChartModel = parsedChart;
        }
    }
}
