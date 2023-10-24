using System.Collections.Generic;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.FlatFile.TextRepository
{
    public interface ICharacterSerializer
    {
        CharacterModel Deserialize(IChartRepository businessObject, IEnumerable<string> lines);
        CharacterModel Deserialize(IChartRepository businessObject, string text);
        string Serialize(CharacterModel character);
    }
}