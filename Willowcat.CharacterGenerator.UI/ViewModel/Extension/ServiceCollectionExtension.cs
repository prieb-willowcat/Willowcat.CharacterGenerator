using Microsoft.Extensions.DependencyInjection;
using System;
using Willowcat.CharacterGenerator.Model.Progress;
using Willowcat.CharacterGenerator.UI.ViewModel.Factory;

namespace Willowcat.CharacterGenerator.UI.ViewModel.Extension
{
    public static class ServiceCollectionExtension
    {
        public static ServiceCollection RegisterViewModels(this ServiceCollection services)
        {
            var chartProgress = new Progress<ChartSetupMessage>();
            services.AddSingleton(chartProgress);
            services.AddSingleton<IProgress<ChartSetupMessage>>(chartProgress);

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
