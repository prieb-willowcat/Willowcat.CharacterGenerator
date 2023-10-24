using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.FlatFile.TextRepository
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
            CharacterModel? model = null;
            if (lines.Any(text => text.Contains("|")))
            {
                model = _NewPipeFileSerializer.Deserialize(businessObject, lines);
            }
            else
            {
                model = _OldFlatFileSerializer.Deserialize(businessObject, lines);
            }
            return PostProcessing(model);
        }

        public CharacterModel Deserialize(IChartRepository businessObject, string text)
        {
            CharacterModel? model = null;
            if (text.Contains("|"))
            {
                model = _NewPipeFileSerializer.Deserialize(businessObject, text);
            }
            else
            {
                model = _OldFlatFileSerializer.Deserialize(businessObject, text);
            }
            return PostProcessing(model);
        }

        private static CharacterModel PostProcessing(CharacterModel model)
        {
            if (!model.Notes.StartsWith("<"))
            {
                model.Notes = $"<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><Paragraph>{model.Notes.Replace("\n", "</Paragraph>\n<Paragraph>")}</Paragraph></FlowDocument>";
            }
            return model;
        }

        public string Serialize(CharacterModel character)
        {
            return _NewPipeFileSerializer.Serialize(character);
        }
    }
}
