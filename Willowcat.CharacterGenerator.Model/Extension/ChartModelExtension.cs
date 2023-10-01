namespace Willowcat.CharacterGenerator.Model.Extension
{
    public static class ChartModelExtension
    {
        public static void AddOption(this ChartModel @this, int start, int end, string description)
        {
            var range = new DiceRange(start, end);
            var option = new OptionModel(@this.Key, Guid.NewGuid(), description, range);
            @this.Options.Add(option);
        }

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

        public static OptionModel? GetOptionForResult(this ChartModel @this, int result)
            => @this.Options.FirstOrDefault(option => option.Range.InsideRange(result));

        public static SelectedOption? GetSelectedOption(this ChartModel @this, Guid optionId)
        {
            SelectedOption? Result = null;
            OptionModel? option = @this.Options.FirstOrDefault(opt => opt.OptionId == optionId);
            if (option != null)
            {
                Result = @this.CreateSelectedOption(option);
            }
            return Result;
        }
    }
}
