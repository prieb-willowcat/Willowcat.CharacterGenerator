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
using Willowcat.CharacterGenerator.UI.ViewModel;

namespace Willowcat.CharacterGenerator.UI.View
{
    /// <summary>
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        public ChartView()
        {
            InitializeComponent();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ChartViewModel viewModel = DataContext as ViewModel.ChartViewModel;
            if (viewModel == null) return;

            viewModel.UseSelectionCommand.Execute(null);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = e.OriginalSource as Hyperlink;
            if (link == null) return;

            ChartViewModel viewModel = DataContext as ChartViewModel;
            if (viewModel == null) return;

            //viewModel.OnNavigateToChartExecute(link.NavigateUri);
        }
    }
}
