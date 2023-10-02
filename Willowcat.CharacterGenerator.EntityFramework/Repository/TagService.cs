using Microsoft.EntityFrameworkCore;
using Willowcat.CharacterGenerator.Core.Data;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core
{
    public class TagService
    {
        private readonly ChartContextFactory _factory;

        public TagService(ChartContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<TagModel>> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<TagModel>? result = null;
            using (ChartContext context = _factory.GetChartContext())
            {
                result = await context.Tags.ToListAsync();
            }
            return result ?? Array.Empty<TagModel>();
        }
    }
}
