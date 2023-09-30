using System.Windows;
using Willowcat.CharacterGenerator.UI.Data;

namespace Willowcat.CharacterGenerator.UI
{
    public class UserInterfaceSettings
    {
        private WindowStatePreferences _MainWindowPreferences = null;

        public WindowStatePreferences MainWindowState
        {
            get
            {
                if (_MainWindowPreferences == null)
                {
                    _MainWindowPreferences = new WindowStatePreferences();
                    LoadMainWindowState();
                }
                return _MainWindowPreferences;
            }
        }

        private void LoadMainWindowState()
        {
            _MainWindowPreferences.Height = Settings.Default.WindowHeight;
            _MainWindowPreferences.Left = Settings.Default.WindowLeft;
            _MainWindowPreferences.Top = Settings.Default.WindowTop;
            _MainWindowPreferences.Width = Settings.Default.WindowWidth;
            _MainWindowPreferences.WindowState = Settings.Default.WindowState;
        }

        public void SaveMainWindowState(Window window)
        {
            MainWindowState.SetFromWindow(window);
            Settings.Default.WindowHeight = _MainWindowPreferences.Height;
            Settings.Default.WindowLeft = _MainWindowPreferences.Left;
            Settings.Default.WindowTop = _MainWindowPreferences.Top;
            Settings.Default.WindowWidth = _MainWindowPreferences.Width;
            Settings.Default.WindowState = _MainWindowPreferences.WindowState;
            Settings.Default.Save();
        }
    }
}
