using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Core.Randomizer;

namespace Willowcat.CharacterGenerator.OnlineGenerators.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterOnlineGenerators(this ServiceCollection services, Func<string> getBehindTheNameApiKey)
        {
            services.AddSingleton(provider => new RandomNameChartFactory(provider, getBehindTheNameApiKey));
            services.AddSingleton<IChartCollectionRepository>(provider => provider.GetRequiredService<RandomNameChartFactory>());
            services.AddSingleton<IAutoGeneratorFactory>(provider => provider.GetRequiredService<RandomNameChartFactory>());
            services.AddTransient<IHttpJsonClient, HttpJsonClient>();
            return services;
        }
    }
}
