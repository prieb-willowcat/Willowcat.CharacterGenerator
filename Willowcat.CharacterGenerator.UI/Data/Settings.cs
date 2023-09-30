using System;
using System.Windows;

namespace Willowcat.CharacterGenerator.UI.Data
{
    public class Settings : System.Configuration.ApplicationSettingsBase
    {

        public static Settings Default { get; } = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("")]
        public string DefaultSaveDirectory
        {
            get
            {
                return ((string)(this["DefaultSaveDirectory"]));
            }
            set
            {
                this["DefaultSaveDirectory"] = value;
            }
        }

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("")]
        public string LastOpenedFile
        {
            get
            {
                return ((string)(this["LastOpenedFile"]));
            }
            set
            {
                this["LastOpenedFile"] = value;
            }
        }

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("")]
        public string LastOpenedChart
        {
            get
            {
                return ((string)(this["LastOpenedChart"]));
            }
            set
            {
                this["LastOpenedChart"] = value;
            }
        }

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("0")]
        public double? WindowHeight
        {
            get
            {
                return GetDoubleFromSetting("WindowHeight");
            }
            set
            {
                this["WindowHeight"] = value;
            }
        }

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("0")]
        public double? WindowLeft
        {
            get
            {
                return GetDoubleFromSetting("WindowLeft");
            }
            set
            {
                this["WindowLeft"] = value;
            }
        }

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("0")]
        public double? WindowTop
        {
            get
            {
                return GetDoubleFromSetting("WindowTop");
            }
            set
            {
                this["WindowTop"] = value;
            }
        }

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("")]
        public WindowState? WindowState
        {
            get
            {
                WindowState? result = null;
                string windowStateString = this["WindowState"]?.ToString();
                if (!string.IsNullOrEmpty(windowStateString))
                {
                    result = (WindowState)Enum.Parse(typeof(WindowState), windowStateString);
                }
                return result;
            }
            set
            {
                this["WindowState"] = value;
            }
        }

        [global::System.Configuration.UserScopedSetting()]
        [global::System.Diagnostics.DebuggerNonUserCode()]
        [global::System.Configuration.DefaultSettingValue("0")]
        public double? WindowWidth
        {
            get
            {
                return GetDoubleFromSetting("WindowWidth");
            }
            set
            {
                this["WindowWidth"] = value;
            }
        }



        public double? GetDoubleFromSetting(string key)
        {
            double? result = null;
            if (double.TryParse(this[key]?.ToString(), out double value))
            {
                result = value;
            }
            return result;
        }

    }
}
