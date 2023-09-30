using System;
using System.IO;
using Willowcat.Common.Utilities;

namespace Willowcat.CharacterGenerator.Core
{
    public class DatabaseConfiguration
    {
        public string BehindTheNameApiKey { get; set; }
        public bool CanLoadBehindTheNameCharts => !string.IsNullOrEmpty(BehindTheNameApiKey);
        public string DatabaseLocation { get; set; }
        public string ResourcesDirectory { get; set; }

        public string GetResourceDirectory()
        {
            string resourceDirectory = ResourcesDirectory;

            //string DirectoryName = Path.GetFileName(Environment.CurrentDirectory);
            //if (!DirectoryName.EqualsIgnoreCase("debug") && !DirectoryName.EqualsIgnoreCase("release"))
            //{
            //    resourceDirectory = Path.Combine(Environment.CurrentDirectory, "Resources");
            //}

            return resourceDirectory;
        }
    }
}
