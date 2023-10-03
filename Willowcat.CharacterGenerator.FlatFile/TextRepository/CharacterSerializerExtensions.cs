using System.Linq;
using System.IO;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Application.Interface;

namespace Willowcat.CharacterGenerator.FlatFile.TextRepository
{
    public static class CharacterSerializerExtensions
    {
        public static CharacterModel DeserializeFromFile(this ICharacterSerializer serializer, IChartRepository businessObject, string fullpath)
        {
            CharacterModel result = null;
            if (!string.IsNullOrEmpty(fullpath) && File.Exists(fullpath))
            {
                string[] lines = File.ReadAllLines(fullpath);
                if (lines.Any())
                {
                    result = serializer.Deserialize(businessObject, lines);
                }
            }
            return result;
        }
    }
}
