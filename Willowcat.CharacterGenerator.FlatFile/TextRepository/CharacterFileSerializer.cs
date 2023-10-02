using System.Collections.Generic;
using System.Linq;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.TextRepository
{
    public class CharacterFileSerializer : ICharacterSerializer
    {
        private readonly ICharacterSerializer _OldFlatFileSerializer = null;
        private readonly ICharacterSerializer _NewPipeFileSerializer = null;

        public CharacterFileSerializer()
        {
            _OldFlatFileSerializer = new CharacterDetailFlatFileSerializer();
            _NewPipeFileSerializer = new CharacterPipeFileSerializer();
        }

        public CharacterModel Deserialize(IChartRepository businessObject, IEnumerable<string> lines)
        {
            if (lines.Any(text => text.Contains("|")))
            {
                return _NewPipeFileSerializer.Deserialize(businessObject, lines);
            }
            else
            {
                return _OldFlatFileSerializer.Deserialize(businessObject, lines);
            }
        }

        public CharacterModel Deserialize(IChartRepository businessObject, string text)
        {
            if (text.Contains("|"))
            {
                return _NewPipeFileSerializer.Deserialize(businessObject, text);
            }
            else
            {
                return _OldFlatFileSerializer.Deserialize(businessObject, text);
            }
        }

        public string Serialize(CharacterModel character)
        {
            return _NewPipeFileSerializer.Serialize(character);
        }
    }
}
