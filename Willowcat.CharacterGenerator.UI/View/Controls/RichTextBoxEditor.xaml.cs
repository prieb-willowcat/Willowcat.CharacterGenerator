using System.Windows;
using System.Windows.Controls;

namespace Willowcat.CharacterGenerator.UI.View.Controls
{
    /// <summary>
    /// Interaction logic for RichTextBoxEditor.xaml
    /// </summary>
    public partial class RichTextBoxEditor : UserControl
    {
        public RichTextBoxEditor()
        {
            InitializeComponent();
        }

        public string XamlDocument
        {
            get { return (string)GetValue(XamlDocumentProperty); }
            set { SetValue(XamlDocumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XamlDocument.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XamlDocumentProperty =
            DependencyProperty.Register("XamlDocument", typeof(string), typeof(RichTextBoxEditor), new PropertyMetadata(null));

    }
}
