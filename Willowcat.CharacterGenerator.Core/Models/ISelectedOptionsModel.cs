using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public interface ISelectedOptionsModel
    {
        void RemoveOptionRows(IEnumerable<int> rowsIndexes);
        BindingList<SelectedOption> GetSelectedOptions();
        List<KeyValuePair<int, SelectedOption>> MoveSelectedItems(IEnumerable<int> selectedIndexes, int adjust);
    }
}
