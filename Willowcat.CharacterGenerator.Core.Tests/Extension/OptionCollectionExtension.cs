using System.Collections.Generic;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Tests.Extension
{
    public static class OptionCollectionExtension
    {
        public static void Add(this List<OptionModel> list, int startRange, int endRange, string description)
        {
            list.Add(new OptionModel()
            {
                Description = description,
                Range = new DiceRange(startRange, endRange),
            });
        }
    }
}
