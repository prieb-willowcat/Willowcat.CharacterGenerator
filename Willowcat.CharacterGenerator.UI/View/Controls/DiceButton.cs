using System.Windows;
using System.Windows.Controls;

namespace Willowcat.CharacterGenerator.UI.View.Controls
{
    public class DiceButton : Button
    {
        public int DiceSize
        {
            get { return (int)GetValue(DiceSizeProperty); }
            set { SetValue(DiceSizeProperty, value); }
        }

        public static readonly DependencyProperty DiceSizeProperty =
            DependencyProperty.Register("DiceSize", typeof(int), typeof(DiceButton), new PropertyMetadata(1, DiceSizePropertyChanged));

        public static void DiceSizePropertyChanged(DependencyObject @object, DependencyPropertyChangedEventArgs e)
        {
            if (@object is DiceButton b)
            {
                b.Content = b.DiceSize;
                b.CommandParameter = b.DiceSize;
            }
        }
    }
}
