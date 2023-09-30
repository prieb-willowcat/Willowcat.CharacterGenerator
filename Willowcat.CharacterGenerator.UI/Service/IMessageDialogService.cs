using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.CharacterGenerator.UI.Service
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);
        MessageDialogResult ShowOkDialog(string text, string title);
    }

    public enum MessageDialogResult
    {
        Cancel,
        Ok
    }
}
