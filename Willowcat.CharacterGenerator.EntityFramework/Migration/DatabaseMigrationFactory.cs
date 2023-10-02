using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Data;

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
            IProgress<ChartSetupMessage>? progressReporter = _provider.GetService<IProgress<ChartSetupMessage>>();
            yield return new InitialChartDatabaseMigration(_provider.GetRequiredService<IChartCollectionRepository>(), progressReporter);
        }
    }
}
