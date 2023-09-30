using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Willowcat.CharacterGenerator.UI.View
{
    /// <summary>
    /// Interaction logic for ChartListView.xaml
    /// </summary>
    public partial class ChartListView : UserControl
    {
        public ChartListView()
        {
            InitializeComponent();
        }

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // we don't want to have the horizonal scroll changed
            e.Handled = true;
        }
    }
}
