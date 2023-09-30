using System.Windows;
using System.Windows.Documents;

namespace Willowcat.CharacterGenerator.UI.View.Controls
{
    public class RichTextBoxAssistant : DependencyObject
    {
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document",
            typeof(FlowDocument),
            typeof(RichTextBoxAssistant),
            new FrameworkPropertyMetadata());

        public FlowDocument Document
        {
            get => GetValue(DocumentProperty) as FlowDocument;
            set => SetValue(DocumentProperty, value);
        }

    }
}
