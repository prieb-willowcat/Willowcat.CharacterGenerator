namespace Willowcat.CharacterGenerator.Model.Extension
{
    public static class DiceExtension
    {
        public static int Roll(this Dice @this, Random generator, int modifier)
        {
            int total = modifier;
            for (int i = 0; i < @this.Count; i++)
            {
                total += generator.Next(0, @this.DiceSides) + 1;
            }
            return total;
        }

        public static bool InsideRange(this DiceRange @this, int result)
            => result >= @this.Start && result <= @this.End;

        public static string FormatDice(this Dice @this, string? modifierOperator = null, int? modifier = null)
        {
            string diceText = @this.ToString();
            if (!string.IsNullOrEmpty(modifierOperator) && modifier.HasValue)
            {
                diceText += $" {modifierOperator} {modifier}";
            }
            return diceText;
        }
    }
}
