using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Willowcat.CharacterGenerator.UI.Data;
using Willowcat.CharacterGenerator.UI.ViewModel;

namespace Willowcat.CharacterGenerator.UI
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _ViewModel = null;
        private readonly UserInterfaceSettings _UserInterfaceSettings = new UserInterfaceSettings();

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _ViewModel;
        }

        private void CloseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool continueWithOpen = await WarnIfUnsavedChanges();
            if (continueWithOpen)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                CharacterFileOptions characterFileOptions = _ViewModel.CharacterFileOptions;
                openFileDialog.DefaultExt = characterFileOptions.DefaultExtension;
                openFileDialog.Filter = characterFileOptions.FileDialogFilter;

                bool? result = openFileDialog.ShowDialog();
                if (result.HasValue)
                {
                    await _ViewModel.LoadFromFileAsync(openFileDialog.FileName);
                }
            }
        }

        private void NewCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool continueWithOpen = await WarnIfUnsavedChanges();
            if (continueWithOpen)
            {
                _ViewModel.LoadNewCharacter();
            }
        }

        private void SaveAsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string NewFilePath = GetPathToSaveTo();
            if (!string.IsNullOrWhiteSpace(NewFilePath))
            {
                await _ViewModel.SaveCharacterAsync(NewFilePath);
            }
        }

        private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await SaveChanges();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LeftColumnWidth = leftColumn.Width.Value;
            Properties.Settings.Default.RightColumnWidth = rightColumn.Width.Value;

            bool continueWithClose = await WarnIfUnsavedChanges();
            if (!continueWithClose)
            {
                e.Cancel = true;
            }
            else
            {
                _UserInterfaceSettings.SaveMainWindowState(this);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _UserInterfaceSettings.MainWindowState.ApplyToWindow(this);
            if (Properties.Settings.Default.LeftColumnWidth > leftColumn.MinWidth)
            {
                leftColumn.Width = new GridLength(Properties.Settings.Default.LeftColumnWidth);
            }
            if (Properties.Settings.Default.RightColumnWidth > rightColumn.MinWidth)
            {
                rightColumn.Width = new GridLength(Properties.Settings.Default.RightColumnWidth);
            }
            await _ViewModel.LoadDataAsync();
        }

        private string GetPathToSaveTo()
        {
            string path = null;
            CharacterFileOptions characterFileOptions = _ViewModel.CharacterFileOptions;
            if (string.IsNullOrEmpty(characterFileOptions.InitialDirectory))
            {
                characterFileOptions.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = characterFileOptions.DefaultExtension,
                Filter = characterFileOptions.FileDialogFilter,
                InitialDirectory = characterFileOptions.InitialDirectory
            };

            bool? result = saveFileDialog.ShowDialog();
            path = (result.HasValue) ? saveFileDialog.FileName : null;

            return path;
        }

        private async Task SaveChanges()
        {
            string PathToSaveTo = _ViewModel.CharacterDetailsViewModel.CurrentFilePath;
            if (string.IsNullOrEmpty(PathToSaveTo))
            {
                PathToSaveTo = GetPathToSaveTo();
            }

            if (!string.IsNullOrEmpty(PathToSaveTo))
            {
                await _ViewModel.SaveCharacterAsync(PathToSaveTo);
            }
        }

        private async Task<bool> WarnIfUnsavedChanges()
        {
            bool ContinueWithCommand = true;
            if (_ViewModel.CharacterDetailsViewModel.HasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Do you want to save your changes before closing?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Exclamation
                );

                if (result == MessageBoxResult.Yes)
                {
                    await SaveChanges();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    ContinueWithCommand = false;
                }
            }
            return ContinueWithCommand;
        }
    }
}
