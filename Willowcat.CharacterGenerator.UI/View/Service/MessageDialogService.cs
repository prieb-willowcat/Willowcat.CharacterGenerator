using System.Windows;
using Willowcat.CharacterGenerator.UI.Service;

namespace Willowcat.CharacterGenerator.UI.View.Service
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return (result == MessageBoxResult.OK) ? MessageDialogResult.Ok : MessageDialogResult.Cancel;
        }

        public MessageDialogResult ShowOkDialog(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK);
            return MessageDialogResult.Ok;
        }
    }
}
