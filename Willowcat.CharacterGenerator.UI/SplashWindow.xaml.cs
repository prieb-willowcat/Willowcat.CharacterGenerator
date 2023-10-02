using System.Windows;
using System.Windows.Input;
using Willowcat.CharacterGenerator.UI.ViewModel;

namespace Willowcat.CharacterGenerator.UI
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow(InitializeDatabaseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += SplashWindow_Loaded;            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is InitializeDatabaseViewModel viewModel)
            {
                viewModel.CancelLoad();
            }
            DialogResult = false;
        }

        private void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            bool shouldClose = false;
            if (DataContext is InitializeDatabaseViewModel viewModel)
            {
                Cursor = Cursors.Wait;
                shouldClose = await viewModel.LoadDataAsync().ConfigureAwait(false);
            }
            Cursor = Cursors.Arrow;
            if (shouldClose)
            {
                DialogResult = shouldClose;
            }
        }
    }
}
