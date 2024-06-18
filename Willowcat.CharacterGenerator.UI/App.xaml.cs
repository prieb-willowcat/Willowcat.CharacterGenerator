using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.UI.Data;
using Willowcat.CharacterGenerator.UI.Startup;

namespace Willowcat.CharacterGenerator.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider Provider { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var host = Bootstrapper.CreateApp();
            Provider = host.Services;
            var mainWindow = Provider.GetRequiredService<MainWindow>();
            Current.MainWindow = mainWindow;
#if false
            var splashWindow = Provider.GetRequiredService<SplashWindow>();
            if (splashWindow.ShowDialog() ?? false)
            {
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
#else
            mainWindow.Show();            
#endif
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is ChartParsingException chartParsingException)
            {
                StringBuilder builder = new StringBuilder(chartParsingException.Message);
                builder.Append(Environment.NewLine);
                builder.Append($"File Source = '{chartParsingException.ChartModel?.Source}'");
                builder.Append(Environment.NewLine);
                builder.Append($"Chart Name = '{chartParsingException.ChartModel?.ChartName}'");
                builder.Append(Environment.NewLine);
                builder.Append($"Chart Key = '{chartParsingException.ChartModel?.Key}'");
                if (chartParsingException.InnerException != null)
                {
                    builder.Append(Environment.NewLine);
                    builder.Append(chartParsingException.InnerException.ToString());
                }
                builder.Append(Environment.NewLine);
                builder.Append("No charts will be loaded.");
                MessageBox.Show(builder.ToString(), $"Error parsing chart in '{chartParsingException.ChartModel?.Source}'", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Exception is ApiKeyNotFoundException apiKeyNotFoundException)
            {
                MessageBox.Show(apiKeyNotFoundException.Message, $"Error loading charts.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string message = $"Unexpected error occurred.{Environment.NewLine}{e.Exception.Message}{Environment.NewLine}{Environment.NewLine}{e.Exception.StackTrace}";
                MessageBox.Show(message, "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            e.Handled = true;
        }

    }
}
