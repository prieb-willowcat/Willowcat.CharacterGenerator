using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public class SelectedOptionsModel : ISelectedOptionsModel
    {
        private readonly BindingList<SelectedOption> _SelectedOptions = new BindingList<SelectedOption>();
        private readonly ChartService _ChartService = null;

        public ChartModel Chart { get; private set; }

        public SelectedOptionsModel(ChartService chartService, ChartModel chart)
        {
            _ChartService = chartService;
            Chart = chart;
        }

        public BindingList<SelectedOption> GetSelectedOptions() => _SelectedOptions;
        public List<KeyValuePair<int, SelectedOption>> MoveSelectedItems(IEnumerable<int> selectedIndexes, int adjust)
        {
            throw new NotImplementedException();
        }

        public void RemoveOptionRows(IEnumerable<int> rowsIndexes)
        {
            foreach (var index in rowsIndexes)
            {
                _SelectedOptions.Remove(_SelectedOptions[index]);
            }
        }

        //public int AddOption(string chartKey, Guid optionId, int index)
        //{
        //    int AddedAtIndex = 0;
        //    ChartModel chart = _ChartService.GetChart(chartKey);
        //    if (chart != null)
        //    {
        //        OptionModel option = chart.Options.FirstOrDefault(opt => opt.OptionId == optionId);
        //        if (option != null)
        //        {
        //            SelectedOption detail = new SelectedOption()
        //            {
        //                ChartKey = chartKey,
        //                ChartName = chart.ChartName,
        //                Description = option.Description,
        //                Range = option.Range
        //            };
        //            AddedAtIndex = AddOption(detail, index);
        //        }
        //    }
        //    return AddedAtIndex;
        //}
   
        private int AddOption(SelectedOption detail, int index = -1)
        {
            if (index >= 0 && index < _SelectedOptions.Count)
            {
                _SelectedOptions.Insert(index + 1, detail);
                return index + 1;
            }
            else
            {
                _SelectedOptions.Add(detail);
                return _SelectedOptions.Count - 1;
            }
        }
        public void ClearSelections()
        {
            _SelectedOptions.Clear();
        }
    }
}
