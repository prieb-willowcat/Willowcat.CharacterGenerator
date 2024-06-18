using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.FlatFile.Repository;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.Model.Progress;
using Microsoft.Extensions.Options;

namespace Willowcat.CharacterGenerator.FlatFile.Extension
{
    public static class FlatFileServiceRegistration
    {
        public static IServiceCollection RegisterFlatFileServices(this IServiceCollection services)
        {
            services.AddTransient<ICharacterSerializer, CharacterFileSerializer>();
            services.AddTransient<ChartFlatFileSerializer>();
            services.AddTransient<IChartCollectionRepository>(provider =>
            {
                var flatFileConfiguration = provider.GetService<IOptions<FlatFileConfiguration>>()?.Value ?? new FlatFileConfiguration();
                var serializer = provider.GetService<ChartFlatFileSerializer>();
                var progress = provider.GetService<IProgress<ChartSetupMessage>>();
                return new ChartCollectionBuilder(flatFileConfiguration.ResourcesDirectory, serializer, progress);
            });
            return services;
        }
    }
}
