using Willowcat.CharacterGenerator.Core.Randomizer;

namespace Willowcat.CharacterGenerator.BehindTheName.Generator.Json
{
    public class UiName
    {
        public string name;
        public string surname;
        public string gender;
        public string region;

        public string FullName
        {
            get { return $"{name} {surname} ({region})"; }
        }

        public bool IsMatch(Gender inputGender, string? inputRegion)
        {
            bool match = true;
            if (match && inputGender != Gender.Random)
            {
                match = (inputGender == Gender.Male ? "male" : "female") == gender.ToLower();
            }
            if (match && !string.IsNullOrEmpty(inputRegion))
            {
                match = region.Equals(inputRegion, StringComparison.OrdinalIgnoreCase);
            }
            return match;
        }
    }
}
