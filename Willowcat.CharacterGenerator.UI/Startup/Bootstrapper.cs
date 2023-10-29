using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.Events;
using System;
using Willowcat.CharacterGenerator.Application.Extension;
using Willowcat.CharacterGenerator.EntityFramework.Extension;
using Willowcat.CharacterGenerator.FlatFile.Extension;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.OnlineGenerators.Extension;
using Willowcat.CharacterGenerator.UI.View;
using Willowcat.CharacterGenerator.UI.ViewModel;
using Willowcat.CharacterGenerator.UI.ViewModel.Extension;

namespace Willowcat.CharacterGenerator.UI.Startup
{
    public static class Bootstrapper
    {
        public static ServiceProvider CreateApp()
        {
            var services = new ServiceCollection();
            services
                .RegisterAppServices()
                .RegisterApplicationServices()
                .RegisterLogging()
                .RegisterEntityFrameworkServices(builder => builder.UseSqlite($"Data Source={Properties.Settings.Default.DatabaseLocation}"))
                .RegisterFlatFileServices(() => Properties.Settings.Default.ResourcesDirectory)
                .RegisterOnlineGenerators(() => Environment.GetEnvironmentVariable("BehindTheNamesApiKey", EnvironmentVariableTarget.User))
                .RegisterViewModels()
                .RegisterViews();
            return services.BuildServiceProvider();
        }

        private static ServiceCollection RegisterAppServices(this ServiceCollection services)
        {
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton(new Random());
            return services;
        }

        private static ServiceCollection RegisterLogging(this ServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder
                    .ClearProviders()
                    .AddFilter("Willowcat.CharacterGenerator", LogLevel.Debug)
                    .AddFilter("Willowcat.Common", LogLevel.Warning)
                    //.AddDebugConsole()
                    .AddFile("app.log");
            });

            return services;
        }

        private static ServiceCollection RegisterViews(this ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<SplashWindow>();
            services.AddTransient(provider =>
            {
                var viewModel = provider.GetRequiredService<DiceRollViewModel>();
                return new DiceRollerDialog()
                {
                    DataContext = viewModel
                };
            });
            return services;
        }
    }
}
