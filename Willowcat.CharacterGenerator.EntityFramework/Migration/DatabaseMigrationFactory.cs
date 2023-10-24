using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.EntityFramework.Database;
using Willowcat.CharacterGenerator.Model.Progress;

namespace Willowcat.CharacterGenerator.EntityFramework.Migration
{
    public class DatabaseMigrationFactory
    {
        private readonly IServiceProvider _provider;

        public DatabaseMigrationFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<IDatabaseMigration<ChartContext>> GetMigrations()
        {
            var loggerFactory = _provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<InitialChartDatabaseMigration>();
            var progressReporter = _provider.GetService<IProgress<ChartSetupMessage>>();
            var chartCollectionRepositories = _provider.GetServices<IChartCollectionRepository>();
            yield return new InitialChartDatabaseMigration(logger, chartCollectionRepositories, progressReporter);
        }
    }
}
