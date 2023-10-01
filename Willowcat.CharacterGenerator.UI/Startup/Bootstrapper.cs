using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using System;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Data;
using Willowcat.CharacterGenerator.Core.TextRepository;
using Willowcat.CharacterGenerator.UI.ViewModel;

namespace Willowcat.CharacterGenerator.UI.Startup
{
    public static class Bootstrapper
    {
        public static ServiceProvider CreateApp()
        {
            var services = new ServiceCollection();
            services
                .RegisterConfigurations()
                .RegisterAppServices()
                .RegisterViewModels()
                .RegisterViews();
            return services.BuildServiceProvider();
        }

        private static ServiceCollection RegisterAppServices(this ServiceCollection services)
        {
            services.AddTransient<ICharacterSerializer, CharacterFileSerializer>();
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton<ChartService>();
            services.AddTransient<TagService>(); 
            services.AddSingleton(new Random());
            return services;
        }

        private static ServiceCollection RegisterConfigurations(this ServiceCollection services)
        {
            services.AddSingleton(App.DatabaseConfiguration);
            return services;
        }

        private static ServiceCollection RegisterViewModels(this ServiceCollection services)
        {
            services.AddTransient<ChartListViewModel>();
            services.AddTransient<ChartHistoryViewModel>();
            services.AddTransient<ChartViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<InitializeDatabaseViewModel>();
            return services;
        }

        private static ServiceCollection RegisterViews(this ServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<SplashWindow>();
            return services;
        }
    }
}
