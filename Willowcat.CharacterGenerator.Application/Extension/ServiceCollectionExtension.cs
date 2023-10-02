using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Randomizer;

namespace Willowcat.CharacterGenerator.Application.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterApplicationServices(this ServiceCollection services)
        {
            services.AddSingleton(provider => new MythicAutoGeneratorFactory());
            services.AddSingleton<IChartCollectionRepository>(provider => provider.GetRequiredService<MythicAutoGeneratorFactory>());
            services.AddSingleton<IAutoGeneratorFactory>(provider => provider.GetRequiredService<MythicAutoGeneratorFactory>());
            services.AddSingleton(new Random());
            return services;
        }
    }
}
