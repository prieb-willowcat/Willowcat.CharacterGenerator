using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.UI.ViewModel;

namespace Willowcat.CharacterGenerator.UI.View
{
    /// <summary>
    /// Interaction logic for CharacterDetailsView.xaml
    /// </summary>
    public partial class CharacterDetailsView : UserControl
    {

        public CharacterDetailsView()
        {
            InitializeComponent();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not CharacterDetailsViewModel viewModel) return;

            if (sender is not DataGridCell dataGridCellTarget) return;

            if (dataGridCellTarget.DataContext is not SelectedOption option) return;

            string columnHeader = dataGridCellTarget.Column.Header?.ToString();
            if (columnHeader == "Chart")
            {
                viewModel.OnNavigateToSelectedItemExecute(option);
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            IEditableCollectionView itemsView = CollectionViewSource.GetDefaultView(detailOptionsDataGrid.Items) as IEditableCollectionView;
            bool isDeleteKey = (e.Key == Key.Delete || e.Key == Key.Back);
            if (isDeleteKey && !itemsView.IsAddingNew && !itemsView.IsEditingItem)
            {
                if (detailOptionsDataGrid.IsKeyboardFocusWithin)
                {
                    
                }
                //FireItemsRemovedFromDataGrid();
            }
        }
    }
}
