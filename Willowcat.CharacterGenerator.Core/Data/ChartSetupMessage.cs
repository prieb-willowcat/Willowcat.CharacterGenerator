using System;

namespace Willowcat.CharacterGenerator.Core.Data
{
    public class ChartSetupMessage
    {
        public Exception Error { get; set; }
        public int? CurrentProgress { get; }
        public int? MaximumProgress { get; }
        public string Message { get; }
        public object Migration { get; }

        public ChartSetupMessage(object migration, string message)
        {
            Migration = migration ?? throw new ArgumentNullException(nameof(migration));
            Message = message;
        }

        public ChartSetupMessage(object migration, string message, Exception ex)
        {
            Migration = migration ?? throw new ArgumentNullException(nameof(migration));
            Message = message;
            Error = ex;
        }

        public ChartSetupMessage(object migration, int current)
            : this(migration, null)
        {
            CurrentProgress = current;
        }

        public ChartSetupMessage(object migration, int current, int max)
            : this(migration, null)
        {
            CurrentProgress = current;
            MaximumProgress = max;
        }

        public ChartSetupMessage(object migration, string message, int current)
            : this(migration, message)
        {
            CurrentProgress = current;
        }

        public ChartSetupMessage(object migrationName, string message, int current, int max)
            : this(migrationName, message)
        {
            CurrentProgress = current;
            MaximumProgress = max;
        }
    }
}
