using System.Collections.Generic;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.TextRepository
{
    public interface ICharacterSerializer
    {
        CharacterModel Deserialize(IChartRepository businessObject, IEnumerable<string> lines);
        CharacterModel Deserialize(IChartRepository businessObject, string text);
        string Serialize(CharacterModel character);
    }
}