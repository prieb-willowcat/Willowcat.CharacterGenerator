using System.Windows;

namespace Willowcat.CharacterGenerator.UI.Data
{
    public class WindowStatePreferences
    {
        public double? Height { get; set; }

        public double? Left { get; set; }

        public double? Top { get; set; }

        public double? Width { get; set; }

        public WindowState? WindowState { get; set; }


        public WindowStatePreferences()
        {

        }

        public void ApplyToWindow(Window window)
        {
            if (Top.HasValue)
            {
                window.Top = (Top >= SystemParameters.VirtualScreenTop) ? Top.Value : SystemParameters.VirtualScreenTop;
            }
            if (Left.HasValue)
            {
                window.Left = (Left >= SystemParameters.VirtualScreenLeft) ? Left.Value : SystemParameters.VirtualScreenLeft;
            }
            if (Height.HasValue && Height > 0)
            {
                window.Height = (Height <= SystemParameters.VirtualScreenHeight) ? Height.Value : SystemParameters.VirtualScreenHeight;
            }
            if (Width.HasValue && Width > 0)
            {
                window.Width = (Width <= SystemParameters.VirtualScreenWidth) ? Width.Value : SystemParameters.VirtualScreenWidth;
            }
            if (WindowState.HasValue)
            {
                window.WindowState = WindowState.Value;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.WindowState = System.Windows.WindowState.Maximized;
            }
        }

        public void SetFromWindow(Window window)
        {
            if (window.WindowState != System.Windows.WindowState.Minimized)
            {
                Left = window.Left;
                Height = window.Height;
                Top = window.Top;
                Width = window.Width;
                WindowState = window.WindowState;
            }
        }
    }
}
