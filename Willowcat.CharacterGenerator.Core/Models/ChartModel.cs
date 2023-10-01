using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;

namespace Willowcat.CharacterGenerator.Core.Models
{
    [DebuggerDisplay("{ChartName} {Dice} ({Key})")]
    [Table("Chart")]
    public class ChartModel
    {
        [NotMapped]
        public virtual bool CanDynamicallyGenerateOptions { get; protected set; } = false;
        
        [NotMapped]
        public virtual bool ShowRegionSelector { get; protected set; } = false;
        
        public string ChartName { get; set; }
        
        public Dice Dice { get; set; } = new Dice(0, 0);
        
        [Key]
        public string Key { get; set; }
        
        public string Notes { get; set; }

        public List<OptionModel> Options { get; set; } = new List<OptionModel>();

        public ChartModel ParentChart { get; set; }

        public string ParentKey { get; set; }

        [NotMapped]
        public HashSet<string> ParsedTags { get; set; } = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        [NotMapped]
        public Dictionary<string, string> Regions { get; private set; } = new Dictionary<string, string>();

        public int Sequence { get; set; }
        
        public string Source { get; set; }
        
        public List<ChartModel> SubCharts { get; set; } = new List<ChartModel>();

        public List<TagModel> Tags { get; set; } = new List<TagModel>();

        public void AddOption(int start, int end, string description)
        {
            var range = new DiceRange(start, end);
            var option = new OptionModel(Key, Guid.NewGuid(), description, range);
            Options.Add(option);
        }

        public void ClearOptions() => Options.Clear();

        public SelectedOption CreateSelectedOption(OptionModel option)
        {
            return new SelectedOption()
            {
                ChartKey = Key,
                ChartName = ChartName,
                Description = option.Description,
                Range = option.Range
            };
        }

        public virtual Task GenerateOptionsAsync(Random randomizer, string selection)
        {
            return Task.FromResult(0);
        }

        public Dice GestimateDice()
        {
            if (Options.Any())
            {
                return new Dice(1, Options.Max(x => x.Range.End));
            }
            else
            {
                return new Dice();
            }
        }

        public OptionModel GetOptionForResult(int result) => Options.FirstOrDefault(option => option.Range.InsideRange(result));

        public SelectedOption GetSelectedOption(int result)
        {
            SelectedOption Result = null;
            OptionModel option = GetOptionForResult(result);
            if (option != null)
            {
                Result = CreateSelectedOption(option);
            }
            return Result;
        }

        public SelectedOption GetSelectedOption(Guid optionId)
        {
            SelectedOption Result = null;
            OptionModel option = Options.FirstOrDefault(opt => opt.OptionId == optionId);
            if (option != null)
            {
                Result = CreateSelectedOption(option);
            }
            return Result;
        }

        public virtual void LoadSavedOptions(string selection = null)
        {
        }
    }

    [DebuggerDisplay("{Name} (Id={TagId})")]
    public class TagModel
    {
        [Key]
        public int TagId { get; set; }
        public string Name { get; set; }
    }
}
