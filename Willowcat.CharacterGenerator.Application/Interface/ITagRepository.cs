using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Application.Interface
{
    public interface ITagRepository
    {
        Task<IEnumerable<TagModel>> GetTagsAsync(CancellationToken cancellationToken = default);
    }
}