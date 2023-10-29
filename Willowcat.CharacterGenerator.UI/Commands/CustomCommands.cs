using System.Windows.Input;

namespace Willowcat.CharacterGenerator.UI.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand New = new RoutedUICommand(
           "New",
           "New",
           typeof(CustomCommands),
           new InputGestureCollection()
           {
                new KeyGesture(Key.N, ModifierKeys.Control)
           }
       );

        public static readonly RoutedUICommand DiceRoller = new RoutedUICommand(
           "Dice Roller...",
           "DiceRoller",
           typeof(CustomCommands),
           new InputGestureCollection()
           {
                new KeyGesture(Key.F6)
           }
       );
    }
}
