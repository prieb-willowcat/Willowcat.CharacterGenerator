using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using NReco.Logging.File;
using Willowcat.CharacterGenerator.Application.Extension;
using Willowcat.CharacterGenerator.EntityFramework.Extension;
using Willowcat.CharacterGenerator.FlatFile.Extension;

namespace Willowcat.CharacterGenerator.ConsoleApp
{
    public static class Bootstrapper
    {
        private static void BuildConfiguration(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
            configurationBuilder.AddJsonFile("appsettings.json", optional: false);
        }

        public static IHost CreateApp(ConsoleArguments options)
        {
            var hostBuilder = new HostBuilder();
            hostBuilder
                .ConfigureAppConfiguration(BuildConfiguration)
                .ConfigureServices((context, services) => RegisterServices(services));
            return hostBuilder.Build();
        }

        private static IServiceCollection RegisterLogging(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder
                    .ClearProviders()
                    .AddFilter("Willowcat", LogLevel.Debug)
                    .AddFilter("Willowcat.CharacterGenerator", LogLevel.Debug)
                    .AddFilter("Willowcat.Common", LogLevel.Warning)
                    //.AddDebugConsole()
                    .AddProvider(new DebugLoggerProvider())
                    .AddConsole()
                    .AddFile("console.log");
            });

            return services;
        }

        public static ServiceProvider RegisterServices(IServiceCollection services)
        {
            services.AddOptions<DatabaseConfiguration>()
                .BindConfiguration("DatabaseConfiguration")
                .ValidateOnStart();

            services.AddOptions<FlatFileConfiguration>()
                .BindConfiguration("FlatFileConfiguration")
                .ValidateOnStart();

            services
                .RegisterApplicationServices()
                .RegisterLogging()
                .RegisterEntityFrameworkServices()
                .RegisterFlatFileServices();
            return services.BuildServiceProvider();
        }
    }
}
