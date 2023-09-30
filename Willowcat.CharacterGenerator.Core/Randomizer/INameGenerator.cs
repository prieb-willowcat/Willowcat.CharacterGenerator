using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Willowcat.CharacterGenerator.Core.Randomizer
{
    public interface INameGenerator
    {
        IEnumerable<string> GetSavedNames(string selectedRegion);
        Task<IEnumerable<string>> GetNamesAsync(string selectedRegion);
    }
}
