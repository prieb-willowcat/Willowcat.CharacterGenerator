namespace Willowcat.CharacterGenerator.EntityFramework.Migration
{
    public interface IDatabaseMigration<T>
    {
        Task BringDownAsync(T context, CancellationToken cancellationToken);
        Task<bool> BringUpAsync(T context, CancellationToken cancellationToken);
    }
}
