using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.FlatFile.Repository;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.Model.Progress;

namespace Willowcat.CharacterGenerator.FlatFile.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterFlatFileServices(this ServiceCollection services, Func<string> getResourceDirectory)
        {
            services.AddTransient<ICharacterSerializer, CharacterFileSerializer>();
            services.AddTransient<ChartFlatFileSerializer>();
            services.AddTransient<IChartCollectionRepository>(provider =>
            {
                var serializer = provider.GetService<ChartFlatFileSerializer>();
                var progress = provider.GetService<IProgress<ChartSetupMessage>>();
                return new ChartCollectionBuilder(getResourceDirectory(), serializer, progress);
            });
            return services;
        }
    }
}
