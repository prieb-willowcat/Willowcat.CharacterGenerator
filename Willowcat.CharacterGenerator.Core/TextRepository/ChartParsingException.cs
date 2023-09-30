using System;
using Willowcat.CharacterGenerator.Core.Models;

namespace Willowcat.CharacterGenerator.Core.TextRepository
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
