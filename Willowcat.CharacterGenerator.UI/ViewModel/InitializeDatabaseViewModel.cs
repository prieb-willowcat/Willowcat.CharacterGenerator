using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Core.Data;
using Willowcat.CharacterGenerator.EntityFramework.Migration;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class InitializeDatabaseViewModel : ViewModelBase
    {
        private readonly DatabaseMigrationService _databaseMigrationService;
        private readonly object _Lock = new object();
        private readonly object _MessageLock = new object();

        private bool _HasError = false;
        private CancellationTokenSource _TokenSource;
        private int _CurrentProgress = 0;
        private int _MaximumProgress = 100;
        private string _StatusMessage = string.Empty;
        private string _StatusLog = string.Empty;

        protected CancellationTokenSource TokenSource
        {
            get => _TokenSource;
            set
            {
                _TokenSource = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public string DatabaseLocation
        {
            get => Properties.Settings.Default.DatabaseLocation;
            set
            {
                Properties.Settings.Default.DatabaseLocation = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public string ResourcesDirectory
        {
            get => Properties.Settings.Default.ResourcesDirectory;
            set
            {
                Properties.Settings.Default.ResourcesDirectory = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool HasError
        {
            get => _HasError;
            private set
            {
                _HasError = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning => TokenSource != null;

        public int CurrentProgress
        {
            get => _CurrentProgress;
            private set
            {
                _CurrentProgress = value;
                OnPropertyChanged();
            }
        }

        public int MaximumProgress
        {
            get => _MaximumProgress;
            private set
            {
                _MaximumProgress = value;
                OnPropertyChanged();
            }
        }

        public string StatusLog
        {
            get => _StatusLog;
            private set
            {
                _StatusLog = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _StatusMessage;
            private set
            {
                _StatusMessage = value;
                OnPropertyChanged();
            }
        }

        public InitializeDatabaseViewModel()
        {
            StatusMessage = "This is a test message";
            StatusLog = "Willowcat.CharacterGenerator" + Environment.NewLine + StatusMessage;
        }

        public InitializeDatabaseViewModel(Progress<ChartSetupMessage> progressReporter, DatabaseConfiguration configuration, DatabaseMigrationService databaseMigrationService)
        {
            _databaseMigrationService = databaseMigrationService;
            progressReporter.ProgressChanged += ProgressReporter_ProgressChanged;
        }

        public void CancelLoad()
        {
            lock (_Lock)
            {
                TokenSource?.Cancel();
            }
        }

        public async Task<bool> LoadDataAsync()
        {
            lock (_Lock)
            {
                if (TokenSource != null) return false;
            }
            try
            {
                _StatusLog = string.Empty;
                _StatusMessage = string.Empty;

                lock (_Lock)
                {
                    TokenSource = new CancellationTokenSource();
                }
                if (CanLoadDatabase())
                {
                    HasError = !(await InitializeDatabaseAsync(TokenSource.Token));
                }
                else
                {
                    HasError = true;
                    LogMessage("Unable to load database - resources directory could not be found");
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("Database setup was cancelled.");
            }
            catch (Exception ex)
            {
                HasError = true;
                LogError(ex);
            }
            finally
            {
                lock (_Lock)
                {
                    TokenSource = null;
                }
            }
            return !HasError;
        }

        private bool CanLoadDatabase()
        {
            if (!File.Exists(DatabaseLocation))
            {
                var parentPath = Path.GetDirectoryName(DatabaseLocation);
                if (!Directory.Exists(parentPath))
                {
                    Directory.CreateDirectory(parentPath);
                }
            }

            return !string.IsNullOrEmpty(ResourcesDirectory) && Directory.Exists(ResourcesDirectory);
        }

        private async Task<bool> InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            bool success = false;
            //DatabaseMigrationService.ClearOldDatabase(DatabaseLocation);
            if (await _databaseMigrationService.InitializeAsync(cancellationToken))
            {
                success = true;
                LogMessage($"Database is ready.");
            }
            return success;
        }

        private void LogError(Exception ex)
        {
            lock (_MessageLock)
            {
                HasError = true;
                StatusMessage = ex.Message;
                StatusLog += Environment.NewLine + ex;
            }
        }

        private void LogMessage(string message)
        {
            lock (_MessageLock)
            {
                Debug.WriteLine(message);
                StatusMessage = message;
                if (!string.IsNullOrEmpty(StatusLog))
                {
                    StatusLog += Environment.NewLine;
                }
                StatusLog += $"{DateTime.Now:HH:mm:ss.fff} {message}";
            }
        }

        private void ProgressReporter_ProgressChanged(object sender, ChartSetupMessage e)
        {
            if (!string.IsNullOrEmpty(e.Message)) 
            {
                var message = $"{e.Migration.GetType().Name}: {e.Message}";
                LogMessage(message);
                if (e.Error != null)
                {
                    LogError(e.Error);
                }
            }
            if (e.MaximumProgress.HasValue)
            {
                MaximumProgress = e.MaximumProgress.Value;
                //Debug.WriteLine($"{CurrentProgress} / {MaximumProgress}");
            }
            if (e.CurrentProgress.HasValue)
            {
                CurrentProgress = e.CurrentProgress.Value;
                //Debug.WriteLine($"{CurrentProgress} / {MaximumProgress}");
            }
        }
    }
}
