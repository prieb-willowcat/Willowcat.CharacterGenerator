using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using System;
using Willowcat.CharacterGenerator.Application.Extension;
using Willowcat.CharacterGenerator.EntityFramework.Extension;
using Willowcat.CharacterGenerator.FlatFile.Extension;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.OnlineGenerators.Extension;
using Willowcat.CharacterGenerator.UI.ViewModel.Extension;

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
                .RegisterApplicationServices()
                .RegisterEntityFrameworkServices(builder => builder.UseSqlite($"Data Source={Properties.Settings.Default.DatabaseLocation}"))
                .RegisterFlatFileServices(() => Properties.Settings.Default.ResourcesDirectory)
                .RegisterOnlineGenerators(() => Environment.GetEnvironmentVariable("BehindTheNamesApiKey", EnvironmentVariableTarget.User))
                .RegisterViewModels()
                .RegisterViews();
            return services.BuildServiceProvider();
        }

        private static ServiceCollection RegisterAppServices(this ServiceCollection services)
        {
            services.AddTransient<ICharacterSerializer, CharacterFileSerializer>();
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton(new Random());
            return services;
        }

        private static ServiceCollection RegisterConfigurations(this ServiceCollection services)
        {
            services.AddSingleton(App.DatabaseConfiguration);
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
