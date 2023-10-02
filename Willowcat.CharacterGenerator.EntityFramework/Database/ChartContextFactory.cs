using Microsoft.Extensions.DependencyInjection;

namespace Willowcat.CharacterGenerator.Core.Data
{
    public class ChartContextFactory
    {
        private readonly IServiceProvider _provider;

        public ChartContextFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ChartContext GetChartContext() => _provider.GetRequiredService<ChartContext>();
    }
}
