using Willowcat.CharacterGenerator.EntityFramework.Database;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Progress;

namespace Willowcat.CharacterGenerator.EntityFramework.Migration
{

    public class DatabaseMigrationService : IDisposable
    {
        private readonly DatabaseMigrationFactory _migrationFactory;
        private readonly ChartContext _chartContext;
        private readonly IProgress<ChartSetupMessage>? _progressReporter;
        private bool _disposedValue;

        public DatabaseMigrationService(ChartContext chartContext, IProgress<ChartSetupMessage>? progressReporter, DatabaseMigrationFactory migrationFactory)
        {
            _chartContext = chartContext;
            _progressReporter = progressReporter;
            _migrationFactory = migrationFactory;
        }


        private async Task<bool> ApplyMigrationAsync(bool success, int migrationId, IDatabaseMigration<ChartContext> migration, CancellationToken cancellationToken)
        {
            if (!success) return success;

            if (_chartContext.Migrations.Find(migrationId) == null)
            {
                _progressReporter?.Report(new ChartSetupMessage(migration, "Bringing up"));
                success = await migration.BringUpAsync(_chartContext, cancellationToken);
                if (success)
                {
                    await _chartContext.Migrations.AddAsync(new MigrationModel()
                    {
                        Name = migration.GetType().Name,
                        MigrationId = migrationId,
                        DateRan = DateTime.Now,
                    });
                    await _chartContext.SaveChangesAsync();
                }
            }
            return success;
        }

        public static void ClearOldDatabase(string databaseLocation)
        {
            if (File.Exists(databaseLocation))
            {
                File.Delete(databaseLocation);
            }
        }

        public async Task<bool> InitializeAsync(CancellationToken cancellationToken)
        {
            if (await _chartContext.Database.EnsureCreatedAsync(cancellationToken))
            {
                //_progressReporter?.Report($"Database initialized");
            }

            cancellationToken.ThrowIfCancellationRequested();

            bool success = true;
            int index = 0;
            foreach (var migration in _migrationFactory.GetMigrations())
            {
                index++;
                success = await ApplyMigrationAsync(success, index, migration, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            return success;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _chartContext.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
