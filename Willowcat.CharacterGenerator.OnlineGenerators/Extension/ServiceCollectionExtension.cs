using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.OnlineGenerators.Http;

namespace Willowcat.CharacterGenerator.OnlineGenerators.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterOnlineGenerators(this ServiceCollection services, Func<string> getBehindTheNameApiKey)
        {
            services.AddSingleton(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<RandomNameChartFactory>();
                return new RandomNameChartFactory(provider, logger, getBehindTheNameApiKey);
            });
            services.AddSingleton<IChartCollectionRepository>(provider => provider.GetRequiredService<RandomNameChartFactory>());
            services.AddSingleton<IAutoGeneratorFactory>(provider => provider.GetRequiredService<RandomNameChartFactory>());
            services.AddTransient<IHttpJsonClient, HttpJsonClient>();
            return services;
        }
    }
}
