using System.Text.RegularExpressions;

namespace Willowcat.CharacterGenerator.Model
{
    public struct Dice
    {
        private readonly static Regex _dicePattern = new(@"^(\d+)?d(\d+)$");

        public int Count;
        public int DiceSides;

        public readonly int MaximumRoll => Count * DiceSides;

        public readonly int MinimumRoll => Count;

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

        public override readonly bool Equals(object? obj)
        {
            if (obj is Dice dice)
            {
                return Count == dice.Count & DiceSides == dice.DiceSides;
            }
            else
            {
                return false;
            }
        }

        public override readonly int GetHashCode() => Count + DiceSides * 43;

        public static Dice Parse(string value)
        {
            Dice dice = new();
            Match match = _dicePattern.Match(value.Trim());
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
            else
            {
                throw new FormatException($"'{value}' could not be parsed as {nameof(Dice)}. Expected format is '#?d#'");
            }
            return dice;
        }

        public override readonly string ToString() => $"{Count}d{DiceSides}";

        public static bool operator ==(Dice left, Dice right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Dice left, Dice right)
        {
            return !(left == right);
        }
    }
}
