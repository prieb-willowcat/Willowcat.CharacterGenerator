using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.Application.Mythic;

namespace Willowcat.CharacterGenerator.Application.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(provider => new MythicAutoGeneratorFactory());
            services.AddSingleton<IChartCollectionRepository>(provider => provider.GetRequiredService<MythicAutoGeneratorFactory>());
            services.AddSingleton<IAutoGeneratorFactory>(provider => provider.GetRequiredService<MythicAutoGeneratorFactory>());
            services.AddSingleton(new Random());
            return services;
        }
    }
}
