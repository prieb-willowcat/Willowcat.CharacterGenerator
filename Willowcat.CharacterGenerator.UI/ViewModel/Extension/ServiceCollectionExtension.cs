using Microsoft.Extensions.DependencyInjection;
using Willowcat.CharacterGenerator.UI.ViewModel.Factory;

namespace Willowcat.CharacterGenerator.UI.ViewModel.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterViewModels(this ServiceCollection services)
        {
            services.AddSingleton<ChartViewModelFactory>();
            services.AddTransient<ChartListViewModel>();
            services.AddTransient<ChartHistoryViewModel>();
            services.AddTransient<ChartViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<InitializeDatabaseViewModel>();
            return services;
        }
    }
}
