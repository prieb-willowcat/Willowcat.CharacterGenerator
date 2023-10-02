using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Data;
using Willowcat.CharacterGenerator.Core.TextRepository;

namespace Willowcat.CharacterGenerator.FlatFile.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterFlatFileServices(this ServiceCollection services, Func<string> getResourceDirectory)
        {
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
