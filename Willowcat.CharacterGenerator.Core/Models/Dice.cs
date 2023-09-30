using System;
using System.Text.RegularExpressions;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public struct Dice
    {
        private readonly static Regex _DicePattern = new Regex(@"(\d+)?d(\d+)");

        public int Count;
        public int DiceSides;

        public int MaximumRoll => (Count * DiceSides);

        public int MinimumRoll => Count;

        public Dice()
        {
            Count = 0;
            DiceSides = 0;
        }

        public Dice(int count, int sides)
        {
            Count = count;
            DiceSides = sides;
        }

        public int Roll(Random generator, int modifier)
        {
            int total = modifier;
            for (int i = 0; i < Count; i++)
            {
                total += generator.Next(0, DiceSides) + 1;
            }
            return total;
        }

        public static Dice Parse(string value)
        {
            Dice dice = new Dice();
            Match match = _DicePattern.Match(value);
            if (match.Success)
            {
                int count = 1;
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    count = int.Parse(match.Groups[1].Value);
                }
                int diceSides = int.Parse(match.Groups[2].Value);
                dice = new Dice(count, diceSides);
            }
            return dice;
        }

        public override string ToString()
        {
            return $"{Count}d{DiceSides}";
        }
    }
}
