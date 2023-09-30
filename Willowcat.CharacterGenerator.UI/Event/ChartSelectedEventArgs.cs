namespace Willowcat.CharacterGenerator.UI.Event
{
    public class ChartSelectedEventArgs
    {
        public string ChartKey { get; private set; }

        /// <summary>
        /// Defaults to true
        /// </summary>
        public bool ResetNavigationHistory { get; private set; } = true;

        public int? Range { get; set; }

        public ChartSelectedEventArgs(string chartKey)
        {
            ChartKey = chartKey;
        }

        public ChartSelectedEventArgs(string chartKey, bool resetNavigationHistory)
        {
            ChartKey = chartKey;
            ResetNavigationHistory = resetNavigationHistory;
        }

        public ChartSelectedEventArgs(string chartKey, int range)
        {
            ChartKey = chartKey;
            Range = range;
        }
    }
}
