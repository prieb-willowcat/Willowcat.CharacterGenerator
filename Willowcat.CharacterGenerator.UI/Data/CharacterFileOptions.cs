using System.IO;

namespace Willowcat.CharacterGenerator.UI.Data
{
    public class CharacterFileOptions
    {
        public string DefaultExtension => ".txt";

        /// <summary>
        /// Filter string should contain a description of the filter, followed by a vertical bar and the filter pattern. 
        /// Must also separate multiple filter description and pattern pairs by a vertical bar. 
        /// Must separate multiple extensions in a filter pattern with a semicolon. 
        /// 
        /// Example: \"Image files (*.bmp, *.jpg)|*.bmp;*.jpg|All files (*.*)|*.*\"'
        /// </summary>
        public string FileDialogFilter => "Character files (*.txt)|*.txt|All files (*.*)|*.*";

        public string InitialDirectory
        {
            get
            {
                return Settings.Default.DefaultSaveDirectory;
            }
            set
            {
                Settings.Default.DefaultSaveDirectory = value;
                Settings.Default.Save();
            }
        }

        public string LastOpenedChart
        {
            get
            {
                return Settings.Default.LastOpenedChart;
            }
            set
            {
                Settings.Default.LastOpenedChart = value;
                Settings.Default.Save();
            }
        }

        public string LastOpenedFile
        {
            get
            {
                return Settings.Default.LastOpenedFile;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string directory = Path.GetDirectoryName(value);
                    Settings.Default.DefaultSaveDirectory = directory;
                }
                Settings.Default.LastOpenedFile = value;
                Settings.Default.Save();
            }
        }
    }
}
