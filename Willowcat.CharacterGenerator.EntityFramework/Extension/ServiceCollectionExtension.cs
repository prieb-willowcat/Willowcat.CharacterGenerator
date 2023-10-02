using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Data;
using Willowcat.CharacterGenerator.EntityFramework.Migration;

namespace Willowcat.CharacterGenerator.EntityFramework.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterEntityFrameworkServices(this ServiceCollection services, Action<DbContextOptionsBuilder>? dbOptionsAction)
        {
            services.AddDbContext<ChartContext>(dbOptionsAction, ServiceLifetime.Transient, ServiceLifetime.Transient);
            services.AddTransient<ChartContextFactory>();
            services.AddSingleton<DatabaseMigrationFactory>();
            services.AddSingleton<DatabaseMigrationService>();

            services.AddSingleton<ChartService>();
            services.AddSingleton<IChartRepository>(provider => provider.GetRequiredService<ChartService>());

            services.AddTransient<TagService>();
            return services;
        }
    }
}
