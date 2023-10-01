using System;
using System.Linq;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public static class ChartModelExtension
    {
        public static void AddOption(this ChartModel @this, int start, int end, string description)
        {
            var range = new DiceRange(start, end);
            var option = new OptionModel(@this.Key, Guid.NewGuid(), description, range);
            @this.Options.Add(option);
        }

        public static void ClearOptions(this ChartModel @this) => @this.Options.Clear();

        public static SelectedOption CreateSelectedOption(this ChartModel @this, OptionModel option)
        {
            return new SelectedOption()
            {
                ChartKey = @this.Key,
                ChartName = @this.ChartName,
                Description = option.Description,
                Range = option.Range
            };
        }

        public static SelectedOption GetSelectedOption(this ChartModel @this, int result)
        {
            SelectedOption Result = null;
            OptionModel option = @this.GetOptionForResult(result);
            if (option != null)
            {
                Result = @this.CreateSelectedOption(option);
            }
            return Result;
        }

        public static Dice GestimateDice(this ChartModel @this)
        {
            if (@this.Options.Any())
            {
                return new Dice(1, @this.Options.Max(x => x.Range.End));
            }
            else
            {
                return new Dice();
            }
        }

        public static OptionModel GetOptionForResult(this ChartModel @this, int result) 
            => @this.Options.FirstOrDefault(option => option.Range.InsideRange(result));

        public static SelectedOption GetSelectedOption(this ChartModel @this, Guid optionId)
        {
            SelectedOption Result = null;
            OptionModel option = @this.Options.FirstOrDefault(opt => opt.OptionId == optionId);
            if (option != null)
            {
                Result = @this.CreateSelectedOption(option);
            }
            return Result;
        }
    }
}
