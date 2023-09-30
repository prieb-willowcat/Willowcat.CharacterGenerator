using System.Text.RegularExpressions;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public struct DiceRange
    {
        private readonly static Regex _RangePattern = new Regex(@"(\-?\d+)-?(\d+)?");

        public int End;
        public int Start;

        public DiceRange(int start, int end)
        {
            Start = start <= end ? start : end;
            End = end >= start ? end : start;
        }

        public DiceRange(int value)
        {
            Start = value;
            End = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is DiceRange range)
            {
                return range.Start == Start && range.End == End;
            }
            return false;
        }

        public override int GetHashCode() => Start * 17 + End;
        public bool Matches(int result) => result >= Start && result <= End;
        public static DiceRange Parse(string str)
        {
            DiceRange result = new DiceRange(0);

            Match match = _RangePattern.Match(str);
            if (match.Success)
            {
                int start = int.Parse(match.Groups[1].Value);
                int end = start;
                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    end = int.Parse(match.Groups[2].Value);
                }
                result = new DiceRange(start, end);
            }

            return result;
        }
        public override string ToString() => (Start == End) ? Start.ToString() : $"{Start}-{End}";
    }
}
