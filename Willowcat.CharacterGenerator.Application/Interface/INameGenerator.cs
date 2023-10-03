using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Willowcat.CharacterGenerator.Application.Interface
{
    public interface INameGenerator
    {
        bool ShowRegionSelector { get; }
        IEnumerable<string> GetSavedNames(string selectedRegion);
        Task<IEnumerable<string>> GetNamesAsync(string selectedRegion);
    }
}
