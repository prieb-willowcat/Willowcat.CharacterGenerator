using Microsoft.Extensions.DependencyInjection;
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
            var progressReporter = _provider.GetService<IProgress<ChartSetupMessage>>();
            var chartCollectionRepositories = _provider.GetServices<IChartCollectionRepository>();
            yield return new InitialChartDatabaseMigration(chartCollectionRepositories, progressReporter);
        }
    }
}
