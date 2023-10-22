using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Willowcat.CharacterGenerator.UI.View.Behaviors
{
    public class XmlRichTextBoxBehavior : DependencyObject
    {
        private static readonly HashSet<Thread> _recursionProtection = new HashSet<Thread>();

        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(string),
            typeof(XmlRichTextBoxBehavior),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = OnValueChanged
            });
        public static string GetDocumentXaml(DependencyObject richTextBox) => (string)richTextBox.GetValue(DocumentXamlProperty);
        public static void SetDocumentXaml(DependencyObject richTextBox, string value)
        {
            _recursionProtection.Add(Thread.CurrentThread);
            richTextBox.SetValue(DocumentXamlProperty, value);
            _recursionProtection.Remove(Thread.CurrentThread);
        }

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (_recursionProtection.Contains(Thread.CurrentThread)) return;

            var richTextBox = (RichTextBox)dependencyObject;
            var text = GetDocumentXaml(richTextBox);
            var flowDocument = new FlowDocument();
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    flowDocument = XamlReader.Parse(text) as FlowDocument;
                }
            }
            catch (Exception ex)
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Span(new Run($"Error loading text: {ex.Message}"))
                {
                    Foreground = new SolidColorBrush(Colors.Red)
                });
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(new Run(text));
                flowDocument.Blocks.Add(paragraph);
            }
            richTextBox.Document = flowDocument;
            richTextBox.LostFocus -= RichTextBox_LostFocus;
            richTextBox.LostFocus += RichTextBox_LostFocus;
            //richTextBox.TextChanged -= OnTextChanged;
            //richTextBox.TextChanged += OnTextChanged;
        }

        private static void RichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox.Document));
            }
        }

        private static void OnTextChanged(object obj, TextChangedEventArgs e)
        {
            if (obj is RichTextBox richTextBox)
            {
                SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox.Document));
            }
        }
    }
}
