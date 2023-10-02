using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core
{
    public interface ITagRepository
    {
        Task<IEnumerable<TagModel>> GetTagsAsync(CancellationToken cancellationToken = default);
    }
}