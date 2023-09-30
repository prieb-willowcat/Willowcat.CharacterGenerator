using System.Linq;

namespace CharactorGenerator.Common
{
    public static class EnglishGrammar
    {
        public static readonly char[] Vowels = new char[]
        {
            'a','e','i','o','u'
        };

        public static string GetIndeterminateArticle(string word, bool capitalize = false)
        {
            string Result = string.Empty;
            word = word?.Trim()?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(word))
            {
                if (Vowels.Contains(word[0]))
                {
                    Result = capitalize ? "An" : "an";
                }
                else
                {
                    Result = capitalize ? "A" : "a";
                }

            }
            return Result;
        }
    }
}