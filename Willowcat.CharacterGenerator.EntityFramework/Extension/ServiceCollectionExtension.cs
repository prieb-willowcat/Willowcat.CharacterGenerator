using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.EntityFramework.Database;
using Willowcat.CharacterGenerator.EntityFramework.Migration;
using Willowcat.CharacterGenerator.EntityFramework.Repository;

namespace Willowcat.CharacterGenerator.EntityFramework.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterEntityFrameworkServices(this IServiceCollection services)
        {
            services.AddDbContext<ChartContext>((provider, builder) => 
            {
                var options = provider.GetService<IOptions<DatabaseConfiguration>>();
                var dbConfiguration = options?.Value ?? new DatabaseConfiguration();
                var connectionString = $"Data Source={dbConfiguration.DatabaseLocation}";
                builder.UseSqlite(connectionString);
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);
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
