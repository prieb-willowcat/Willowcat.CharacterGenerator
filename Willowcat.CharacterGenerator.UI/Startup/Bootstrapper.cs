using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NReco.Logging.File;
using Prism.Events;
using System;
using Willowcat.CharacterGenerator.Application.Extension;
using Willowcat.CharacterGenerator.EntityFramework.Extension;
using Willowcat.CharacterGenerator.FlatFile.Extension;
using Willowcat.CharacterGenerator.OnlineGenerators.Extension;
using Willowcat.CharacterGenerator.UI.View;
using Willowcat.CharacterGenerator.UI.ViewModel;
using Willowcat.CharacterGenerator.UI.ViewModel.Extension;

namespace Willowcat.CharacterGenerator.UI.Startup
{
    public static class Bootstrapper
    {
        private static void BuildConfiguration(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
            configurationBuilder.AddJsonFile("appsettings.json", optional: false);
        }

        public static IHost CreateApp()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder
                .ConfigureAppConfiguration(BuildConfiguration)
                .ConfigureServices((context, services) => RegisterServices(services));
            return hostBuilder.Build();
        }

        public static IServiceProvider RegisterServices(IServiceCollection services)
        {
            services.AddOptions<DatabaseConfiguration>()
                .BindConfiguration("DatabaseConfiguration")
                .ValidateOnStart();

            services.AddOptions<FlatFileConfiguration>()
                .BindConfiguration("FlatFileConfiguration")
                .ValidateOnStart();

            services
                .RegisterAppServices()
                .RegisterApplicationServices()
                .RegisterLogging()
                .RegisterEntityFrameworkServices()
                .RegisterFlatFileServices()
                .RegisterOnlineGenerators(() => Environment.GetEnvironmentVariable("BehindTheNamesApiKey", EnvironmentVariableTarget.User))
                .RegisterViewModels()
                .RegisterViews();
            return services.BuildServiceProvider();
        }

        private static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton(new Random());
            return services;
        }

        private static IServiceCollection RegisterLogging(this IServiceCollection services)
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

        private static IServiceCollection RegisterViews(this IServiceCollection services)
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
