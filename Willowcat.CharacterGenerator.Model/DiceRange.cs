using System.Text.RegularExpressions;

namespace Willowcat.CharacterGenerator.Model
{
    public struct DiceRange
    {
        private readonly static Regex _rangePattern = new(@"^(\-?\d+)(?:-(\-?\d+))?$");

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

        public override readonly bool Equals(object? obj)
        {
            if (obj is DiceRange range)
            {
                return range.Start == Start && range.End == End;
            }
            return false;
        }

        public override readonly int GetHashCode() => Start * 197 + End;

        public static DiceRange Parse(string str)
        {
            DiceRange result = new(0);

            Match match = _rangePattern.Match(str);
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
            else
            {
                throw new FormatException($"'{str}' could not be parsed as {nameof(DiceRange)}. Expected formats are '#-#' or '#'");
            }

            return result;
        }

        public override readonly string ToString() => Start == End ? Start.ToString() : $"{Start}-{End}";

        public static bool operator ==(DiceRange left, DiceRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiceRange left, DiceRange right)
        {
            return !(left == right);
        }
    }
}
