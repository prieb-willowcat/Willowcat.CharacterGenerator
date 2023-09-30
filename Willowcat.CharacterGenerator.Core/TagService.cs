using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Core.Data;
using Willowcat.CharacterGenerator.Core.Models;

namespace Willowcat.CharacterGenerator.Core
{
    public class TagService
    {
        private readonly DatabaseConfiguration _Options;

        public TagService(DatabaseConfiguration options)
        {
            _Options = options;
        }

        public async Task<IEnumerable<TagModel>> GetTagsAsync()
        {
            List<TagModel> result = null;
            using (ChartContext context = new ChartContext(_Options))
            {
                result = await context.Tags.ToListAsync();
            }
            return result;
        }
    }
}
